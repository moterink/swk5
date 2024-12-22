using Npgsql;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public RouteRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Route>> GetAllAsync()
        {
            var routes = new List<Route>();
            var query = @"SELECT r.id, r.number, r.valid_from, r.valid_to, r.days_of_operation, 
                                rs.stop_id, rs.sequence_number, rs.scheduled_departure_time, 
                                s.name AS stop_name
                          FROM routes r
                          LEFT JOIN route_stops rs ON r.id = rs.route_id
                          LEFT JOIN stops s ON rs.stop_id = s.id
                          ORDER BY r.id, rs.sequence_number";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            Route? currentRoute = null;
            while (await reader.ReadAsync())
            {
                var routeId = reader.GetInt32(0);

                if (currentRoute == null || currentRoute.Id != routeId)
                {
                    currentRoute = new Route
                    {
                        Id = routeId,
                        Number = reader.GetString(1),
                        ValidFrom = DateOnly.FromDateTime(reader.GetDateTime(2)),
                        ValidTo = DateOnly.FromDateTime(reader.GetDateTime(3)),
                        DaysOfOperation = reader.GetString(4),
                        Stops = new List<RouteStop>()
                    };
                    routes.Add(currentRoute);
                }

                if (!reader.IsDBNull(5)) // Ensure the stop exists
                {
                    var stop = new RouteStop
                    {
                        RouteId = routeId,
                        StopId = reader.GetInt32(5),
                        SequenceNumber = reader.GetInt32(6),
                        ScheduledDepartureTime = reader.GetFieldValue<TimeOnly>(7),
                        //StopName = reader.IsDBNull(8) ? null : reader.GetString(8)
                    };

                    currentRoute.Stops.Add(stop);
                }
            }

            return routes;
        }

        public async Task<Route?> GetByIdAsync(int id)
        {
            var query = @"SELECT r.id, r.number, r.valid_from, r.valid_to, r.days_of_operation, 
                                rs.stop_id, rs.sequence_number, rs.scheduled_departure_time, 
                                s.name AS stop_name
                          FROM routes r
                          LEFT JOIN route_stops rs ON r.id = rs.route_id
                          LEFT JOIN stops s ON rs.stop_id = s.id
                          WHERE r.id = @id
                          ORDER BY rs.sequence_number";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();

            Route? route = null;
            while (await reader.ReadAsync())
            {
                if (route == null)
                {
                    route = new Route
                    {
                        Id = reader.GetInt32(0),
                        Number = reader.GetString(1),
                        ValidFrom = DateOnly.FromDateTime(reader.GetDateTime(2)),
                        ValidTo = DateOnly.FromDateTime(reader.GetDateTime(3)),
                        DaysOfOperation = reader.GetString(4),
                        Stops = new List<RouteStop>()
                    };
                }

                if (!reader.IsDBNull(5))
                {
                    var stop = new RouteStop
                    {
                        RouteId = reader.GetInt32(0),
                        StopId = reader.GetInt32(5),
                        SequenceNumber = reader.GetInt32(6),
                        ScheduledDepartureTime = TimeOnly.FromDateTime(reader.GetDateTime(7)),
                        //StopName = reader.IsDBNull(8) ? null : reader.GetString(8)
                    };

                    route.Stops.Add(stop);
                }
            }

            return route;
        }

        public async Task<int> AddAsync(Route route)
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // Insert route
                var routeQuery = @"
                INSERT INTO routes (number, valid_from, valid_to, days_of_operation) 
                VALUES (@number, @validFrom, @validTo, @daysOfOperation) 
                RETURNING id";

                await using var routeCmd = new NpgsqlCommand(routeQuery, conn);
                routeCmd.Parameters.AddWithValue("@number", route.Number);
                routeCmd.Parameters.AddWithValue("@validFrom", route.ValidFrom);
                routeCmd.Parameters.AddWithValue("@validTo", route.ValidTo);
                routeCmd.Parameters.AddWithValue("@daysOfOperation", route.DaysOfOperation);

                var routeId = (int)(await routeCmd.ExecuteScalarAsync());

                // Insert route stops
                var routeStopQuery = @"
                INSERT INTO route_stops (route_id, stop_id, sequence_number, scheduled_departure_time) 
                VALUES (@routeId, @stopId, @sequenceNumber, @scheduledDepartureTime)";

                foreach (var stop in route.Stops)
                {
                    await using var stopCmd = new NpgsqlCommand(routeStopQuery, conn);
                    stopCmd.Parameters.AddWithValue("@routeId", routeId);
                    stopCmd.Parameters.AddWithValue("@stopId", stop.StopId);
                    stopCmd.Parameters.AddWithValue("@sequenceNumber", stop.SequenceNumber);
                    stopCmd.Parameters.AddWithValue("@scheduledDepartureTime", stop.ScheduledDepartureTime);

                    await stopCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return routeId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> UpdateAsync(Route route)
        {
            var query = "UPDATE routes SET number = @number, valid_from = @validFrom, valid_to = @validTo, days_of_operation = @daysOfOperation WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", route.Id);
            cmd.Parameters.AddWithValue("@number", route.Number);
            cmd.Parameters.AddWithValue("@validFrom", route.ValidFrom);
            cmd.Parameters.AddWithValue("@validTo", route.ValidTo);
            cmd.Parameters.AddWithValue("@daysOfOperation", route.DaysOfOperation);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM routes WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
        
        public async Task<IEnumerable<TimetableResult>> GetTimetableAsync(
            int startStopId,
            int endStopId,
            DateTime dateTime,
            bool isArrivalTime,
            int limit)
        {
            var results = new List<TimetableResult>();

            var query = @"
                SELECT DISTINCT
    r.id AS route_id,
    rs1.stop_id AS start_stop_id,
    rs2.stop_id AS end_stop_id,
    (DATE(@dateTime) + rs1.scheduled_departure_time) AS scheduled_departure_time,
    (DATE(@dateTime) + rs2.scheduled_departure_time) AS scheduled_arrival_time,
    MAX(c1.timestamp) AS real_departure_time,
    MAX(c2.timestamp) AS real_arrival_time
FROM route_stops rs1
INNER JOIN route_stops rs2
    ON rs1.route_id = rs2.route_id
INNER JOIN routes r
    ON r.id = rs1.route_id
LEFT JOIN holidays h
    ON (@dateTime::date >= h.start_date AND @dateTime::date <= h.end_date)
LEFT JOIN checkins c1
    ON rs1.route_id = c1.route_id AND rs1.stop_id = c1.stop_id
LEFT JOIN checkins c2
    ON rs2.route_id = c2.route_id AND rs2.stop_id = c2.stop_id
WHERE rs1.sequence_number < rs2.sequence_number
  AND rs1.stop_id = @startStopId
  AND rs2.stop_id = @endStopId
  AND @dateTime::date BETWEEN r.valid_from AND r.valid_to
  AND (
      (h.is_school_holiday IS NULL AND r.days_of_operation LIKE '%Weekday%') OR
      (h.is_school_holiday = TRUE AND r.days_of_operation LIKE '%Holiday%') OR
      (h.is_school_holiday = FALSE AND r.days_of_operation LIKE '%Weekday%')
  )
GROUP BY 
    r.id, rs1.stop_id, rs2.stop_id, rs1.scheduled_departure_time, rs2.scheduled_departure_time
ORDER BY 
    scheduled_departure_time
LIMIT @limit;

            ";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@startStopId", startStopId);
            cmd.Parameters.AddWithValue("@endStopId", endStopId);
            cmd.Parameters.AddWithValue("@dateTime", dateTime);
            cmd.Parameters.AddWithValue("@limit", limit);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var scheduledDepartureTime = reader.GetDateTime(3);
                var scheduledArrivalTime = reader.GetDateTime(4);
                var realDepartureTime = reader.IsDBNull(5) ? scheduledDepartureTime : reader.GetDateTime(5);
                var realArrivalTime = reader.IsDBNull(6) ? scheduledArrivalTime : reader.GetDateTime(6);

                results.Add(new TimetableResult
                {
                    RouteId = reader.GetInt32(0),
                    StartStopId = reader.GetInt32(1),
                    EndStopId = reader.GetInt32(2),
                    ScheduledDepartureTime = scheduledDepartureTime,
                    ScheduledArrivalTime = scheduledArrivalTime,
                    RealDepartureTime = realDepartureTime,
                    RealArrivalTime = realArrivalTime
                });
            }

            return results;
        }

    }
}
