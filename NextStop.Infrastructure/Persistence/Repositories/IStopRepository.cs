using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public interface IStopRepository
    {
        Task<ICollection<Stop>> GetAllAsync();
        Task<Stop?> GetByIdAsync(int id);
        Task<int> AddAsync(Stop stop);
        Task<int> UpdateAsync(Stop stop);
        Task<int> DeleteAsync(int id);
    }
}