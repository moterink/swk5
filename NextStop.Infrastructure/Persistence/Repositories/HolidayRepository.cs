using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public HolidayRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Holiday>> GetAllAsync()
        {
            var holidays = new List<Holiday>();
            var query = "SELECT id, start_date, end_date, name, is_school_holiday FROM holidays";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                holidays.Add(new Holiday
                {
                    Id = reader.GetInt32(0),
                    StartDate = reader.GetDateTime(1),
                    EndDate = reader.GetDateTime(2),
                    Name = reader.GetString(3),
                    IsSchoolHoliday = reader.GetBoolean(4)
                });
            }

            return holidays;
        }

        public async Task<Holiday?> GetByIdAsync(int id)
        {
            var query = "SELECT id, start_date, end_date, name, is_school_holiday FROM holidays WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Holiday
                {
                    Id = reader.GetInt32(0),
                    StartDate = reader.GetDateTime(1),
                    EndDate = reader.GetDateTime(2),
                    Name = reader.GetString(3),
                    IsSchoolHoliday = reader.GetBoolean(4)
                };
            }

            return null;
        }

        public async Task<int> AddAsync(Holiday holiday)
        {
            var query = @"INSERT INTO holidays (start_date, end_date, name, is_school_holiday) 
                          VALUES (@startDate, @endDate, @name, @isSchoolHoliday) RETURNING id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@startDate", holiday.StartDate);
            cmd.Parameters.AddWithValue("@endDate", holiday.EndDate);
            cmd.Parameters.AddWithValue("@name", holiday.Name);
            cmd.Parameters.AddWithValue("@isSchoolHoliday", holiday.IsSchoolHoliday);

            var id = await cmd.ExecuteScalarAsync();
            return (int)id!;
        }

        public async Task<int> UpdateAsync(Holiday holiday)
        {
            var query = @"UPDATE holidays 
                          SET start_date = @startDate, end_date = @endDate, name = @name, is_school_holiday = @isSchoolHoliday 
                          WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", holiday.Id);
            cmd.Parameters.AddWithValue("@startDate", holiday.StartDate);
            cmd.Parameters.AddWithValue("@endDate", holiday.EndDate);
            cmd.Parameters.AddWithValue("@name", holiday.Name);
            cmd.Parameters.AddWithValue("@isSchoolHoliday", holiday.IsSchoolHoliday);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var query = "DELETE FROM holidays WHERE id = @id";

            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
