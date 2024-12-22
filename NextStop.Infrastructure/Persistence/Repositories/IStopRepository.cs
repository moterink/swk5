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
        Task<bool> ExistsByShortNameAsync(string shortName, int excludeId);
        Task<IEnumerable<Stop>> SearchAsync(string query);
        Task<IEnumerable<Departure>> GetNextDeparturesAsync(int stopId, DateTime dateTime, int limit);
    }
}