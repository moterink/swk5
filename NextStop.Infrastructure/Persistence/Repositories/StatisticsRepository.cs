using NextStop.Infrastructure.Persistence.Entities;
using Npgsql;

namespace NextStop.Infrastructure.Persistence.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public StatisticsRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<PunctualityStatistics>> GetPunctualityStatisticsAsync(DateTime startDate, DateTime endDate, string? routeNumber = null)
    {
        var query = @"
            SELECT
                r.number AS route_number,
                ROUND(AVG(EXTRACT(EPOCH FROM (c.timestamp - (DATE(c.timestamp) + rs.scheduled_departure_time)))::NUMERIC), 2) AS avg_delay_seconds,
                COUNT(*) FILTER (WHERE EXTRACT(EPOCH FROM (c.timestamp - (DATE(c.timestamp) + rs.scheduled_departure_time))) < 120) * 100.0 / NULLIF(COUNT(*), 0) AS percent_on_time,
                COUNT(*) FILTER (WHERE EXTRACT(EPOCH FROM (c.timestamp - (DATE(c.timestamp) + rs.scheduled_departure_time))) BETWEEN 120 AND 300) * 100.0 / NULLIF(COUNT(*), 0) AS percent_slightly_delayed,
                COUNT(*) FILTER (WHERE EXTRACT(EPOCH FROM (c.timestamp - (DATE(c.timestamp) + rs.scheduled_departure_time))) BETWEEN 300 AND 600) * 100.0 / NULLIF(COUNT(*), 0) AS percent_delayed,
                COUNT(*) FILTER (WHERE EXTRACT(EPOCH FROM (c.timestamp - (DATE(c.timestamp) + rs.scheduled_departure_time))) > 600) * 100.0 / NULLIF(COUNT(*), 0) AS percent_heavily_delayed
            FROM
                checkins c
                    INNER JOIN
                routes r ON c.route_id = r.id
                    INNER JOIN
                route_stops rs ON c.route_id = rs.route_id AND c.stop_id = rs.stop_id
            WHERE
                c.timestamp BETWEEN @startDate AND @endDate
                AND (r.number = @routeNumber OR @routeNumber IS NULL)
            GROUP BY
                r.number
            ORDER BY
                r.number;
        ";

        await using var conn = await _dataSource.OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@startDate", startDate);
        cmd.Parameters.AddWithValue("@endDate", endDate);
        cmd.Parameters.AddWithValue("@routeNumber", routeNumber != null ? routeNumber : DBNull.Value);

        var result = new List<PunctualityStatistics>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new PunctualityStatistics
            {
                RouteNumber = reader.GetString(0),
                AverageDelaySeconds = reader.GetDouble(1),
                PercentPunctual = reader.GetDouble(2),
                PercentSlightlyDelayed = reader.GetDouble(3),
                PercentDelayed = reader.GetDouble(4),
                PercentSignificantlyDelayed = reader.GetDouble(5)
            });
        }

        return result;
    }
}
