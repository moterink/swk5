using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
