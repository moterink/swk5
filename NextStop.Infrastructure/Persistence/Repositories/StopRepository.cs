using Npgsql;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public class StopRepository : IStopRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public StopRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICollection<Stop>> GetAllAsync()
        {
            var stops = new List<Stop>();
            var query = "SELECT id, name, short_name, latitude, longitude FROM stops";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                stops.Add(new Stop
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    Latitude = reader.GetDouble(3),
                    Longitude = reader.GetDouble(4)
                });
            }

            return stops;
        }

        public async Task<Stop?> GetByIdAsync(int id)
        {
            var query = "SELECT id, name, short_name, latitude, longitude FROM stops WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Stop
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    Latitude = reader.GetDouble(3),
                    Longitude = reader.GetDouble(4)
                };
            }

            return null;
        }

        public async Task<int> AddAsync(Stop stop)
        {
            var query = "INSERT INTO stops (name, short_name, latitude, longitude) VALUES (@name, @shortName, @latitude, @longitude) RETURNING id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", stop.Name);
            cmd.Parameters.AddWithValue("@shortName", stop.ShortName);
            cmd.Parameters.AddWithValue("@latitude", stop.Latitude);
            cmd.Parameters.AddWithValue("@longitude", stop.Longitude);

            var id = await cmd.ExecuteScalarAsync();
            return (int)id!;
        }

        public async Task<int> UpdateAsync(Stop stop)
        {
            var query = "UPDATE stops SET name = @name, short_name = @shortName, latitude = @latitude, longitude = @longitude WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", stop.Id);
            cmd.Parameters.AddWithValue("@name", stop.Name);
            cmd.Parameters.AddWithValue("@shortName", stop.ShortName);
            cmd.Parameters.AddWithValue("@latitude", stop.Latitude);
            cmd.Parameters.AddWithValue("@longitude", stop.Longitude);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM stops WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
        
        public async Task<bool> ExistsByShortNameAsync(string shortName, int excludeId)
        {
            var query = @"SELECT COUNT(*) FROM stops WHERE short_name = @shortName AND id != @excludeId";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@shortName", shortName);
            cmd.Parameters.AddWithValue("@excludeId", excludeId);

            var count = (long)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
        
        public async Task<IEnumerable<Stop>> SearchAsync(string query)
        {
            var stops = new List<Stop>();
            var sqlQuery = "SELECT id, name, short_name, latitude, longitude FROM stops WHERE name ILIKE @query";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(sqlQuery, conn);
            cmd.Parameters.AddWithValue("@query", $"%{query}%");

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                stops.Add(new Stop
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    Latitude = reader.GetDouble(3),
                    Longitude = reader.GetDouble(4)
                });
            }

            return stops;
        }
        
        public async Task<IEnumerable<Departure>> GetNextDeparturesAsync(int stopId, DateTime dateTime, int limit)
        {
            var departures = new List<Departure>();

            var query = @"
                SELECT DISTINCT
                    r.id AS route_id,
                    r.number AS route_number,
                    rs.scheduled_departure_time,
                    s.name AS end_stop,
                    MAX(c.timestamp) AS last_checkin_timestamp
                FROM route_stops rs
                INNER JOIN routes r
                    ON rs.route_id = r.id
                INNER JOIN stops s
                    ON s.id = (
                        SELECT stop_id
                        FROM route_stops
                        WHERE route_id = r.id
                        ORDER BY sequence_number DESC
                        LIMIT 1
                    )
                LEFT JOIN checkins c
                    ON rs.route_id = c.route_id AND rs.stop_id = c.stop_id
                LEFT JOIN holidays h
                    ON @dateTime::date >= h.start_date AND @dateTime::date <= h.end_date
                WHERE rs.stop_id = @stopId
                    AND @dateTime::time <= rs.scheduled_departure_time
                    AND @dateTime::date BETWEEN r.valid_from AND r.valid_to
                    AND (
                       (h.is_school_holiday IS NULL AND r.days_of_operation LIKE '%Weekday%') OR
                       (h.is_school_holiday = TRUE AND r.days_of_operation LIKE '%Holiday%') OR
                       (h.is_school_holiday = FALSE AND r.days_of_operation LIKE '%Weekday%')
                   )
                GROUP BY r.id, r.number, rs.scheduled_departure_time, s.name
                ORDER BY rs.scheduled_departure_time
                LIMIT @limit;
            ";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@stopId", stopId);
            cmd.Parameters.AddWithValue("@dateTime", dateTime);
            cmd.Parameters.AddWithValue("@limit", limit);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var scheduledDepartureTime = reader.GetFieldValue<TimeOnly>(2);
                var lastCheckinTimestamp = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

                var delay = lastCheckinTimestamp.HasValue
                    ? (int)(lastCheckinTimestamp.Value - dateTime).TotalMinutes
                    : 0;

                departures.Add(new Departure
                {
                    RouteId = reader.GetInt32(0),
                    RouteNumber = reader.GetString(1),
                    ScheduledDepartureTime = scheduledDepartureTime,
                    RealDepartureTime = scheduledDepartureTime.AddMinutes(delay),
                    EndStop = reader.GetString(3),
                    Delay = delay
                });
            }

            return departures;
        }

    }

}
