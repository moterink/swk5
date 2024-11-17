using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            var query = "SELECT id, number, valid_from, valid_to, days_of_operation FROM routes";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                routes.Add(new Route
                {
                    Id = reader.GetInt32(0),
                    Number = reader.GetString(1),
                    ValidFrom = reader.GetDateTime(2),
                    ValidTo = reader.GetDateTime(3),
                    DaysOfOperation = reader.GetString(4)
                });
            }

            return routes;
        }

        public async Task<Route?> GetByIdAsync(int id)
        {
            var query = "SELECT id, number, valid_from, valid_to, days_of_operation FROM routes WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Route
                {
                    Id = reader.GetInt32(0),
                    Number = reader.GetString(1),
                    ValidFrom = reader.GetDateTime(2),
                    ValidTo = reader.GetDateTime(3),
                    DaysOfOperation = reader.GetString(4)
                };
            }

            return null;
        }

        public async Task<int> AddAsync(Route route)
        {
            var query = "INSERT INTO routes (number, valid_from, valid_to, days_of_operation) VALUES (@number, @validFrom, @validTo, @daysOfOperation) RETURNING id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@number", route.Number);
            cmd.Parameters.AddWithValue("@validFrom", route.ValidFrom);
            cmd.Parameters.AddWithValue("@validTo", route.ValidTo);
            cmd.Parameters.AddWithValue("@daysOfOperation", route.DaysOfOperation);

            var id = await cmd.ExecuteScalarAsync();
            return (int)id!;
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
    }
}
