using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public class CheckinRepository : ICheckinRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public CheckinRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }
        
        public async Task<int> AddCheckinAsync(int routeId, int stopId, DateTime timestamp)
        {
            var query = @"
                INSERT INTO checkins (route_id, stop_id, timestamp)
                VALUES (@routeId, @stopId, @timestamp)
                RETURNING id;
            ";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@routeId", routeId);
            cmd.Parameters.AddWithValue("@stopId", stopId);
            cmd.Parameters.AddWithValue("@timestamp", timestamp);

            return (int)(await cmd.ExecuteScalarAsync());
        }

        public async Task<IEnumerable<Checkin>> GetCheckinsByStopAndRouteAsync(int stopId, int routeId)
        {
            var checkins = new List<Checkin>();
            var query = "SELECT id, route_id, stop_id, timestamp FROM checkins WHERE stop_id = @stopId AND route_id = @routeId";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@stopId", stopId);
            cmd.Parameters.AddWithValue("@routeId", routeId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                checkins.Add(new Checkin
                {
                    Id = reader.GetInt32(0),
                    RouteId = reader.GetInt32(1),
                    StopId = reader.GetInt32(2),
                    Timestamp = reader.GetDateTime(3)
                });
            }

            return checkins;
        }
    }
}
