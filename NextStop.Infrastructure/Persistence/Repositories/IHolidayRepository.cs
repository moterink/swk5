using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public interface IHolidayRepository
    {
        Task<IEnumerable<Holiday>> GetAllAsync();
        Task<Holiday?> GetByIdAsync(int id);
        Task<int> AddAsync(Holiday holiday);
        Task<int> UpdateAsync(Holiday holiday);
        Task<int> DeleteAsync(int id);
    }
}