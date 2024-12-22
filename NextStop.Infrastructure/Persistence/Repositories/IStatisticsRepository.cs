using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Infrastructure.Persistence.Repositories;

public interface IStatisticsRepository
{
    Task<IEnumerable<PunctualityStatistics>> GetPunctualityStatisticsAsync(DateTime startDate, DateTime endDate, string? routeNumber = null);
}