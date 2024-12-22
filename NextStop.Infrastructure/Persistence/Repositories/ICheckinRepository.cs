using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories;

public interface ICheckinRepository
{
    Task<int> AddCheckinAsync(int routeId, int stopId, DateTime timestamp);
    Task<IEnumerable<Checkin>> GetCheckinsByStopAndRouteAsync(int stopId, int routeId);
}