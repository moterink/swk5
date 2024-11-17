using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public interface IRouteRepository
    {
        Task<IEnumerable<Route>> GetAllAsync();
        Task<Route?> GetByIdAsync(int id);
        Task<int> AddAsync(Route route);
        Task<int> UpdateAsync(Route route);
        Task<int> DeleteAsync(int id);
    }
}