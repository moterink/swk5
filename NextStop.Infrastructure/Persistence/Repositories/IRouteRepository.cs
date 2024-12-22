using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories
{
    public interface IRouteRepository
    {
        Task<IEnumerable<Route>> GetAllAsync();
        Task<int> AddAsync(Route route);
        Task<Route?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Route route);
        Task<int> DeleteAsync(int id);

        Task<IEnumerable<TimetableResult>> GetTimetableAsync(int startStopId, int endStopId, DateTime dateTime,
            bool isArrivalTime, int limit);
    }
}
