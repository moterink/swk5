using Microsoft.AspNetCore.Mvc;
using NextStop.Infrastructure.Persistence.Repositories;

namespace NextStop.Controllers;

[ApiController]
[Route("api/statistics")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsRepository _statisticsRepository;

    public StatisticsController(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    [HttpGet("punctuality")]
    public async Task<IActionResult> GetPunctualityStatistics(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate, 
        [FromQuery] string? routeNumber = null)
    {
        if (startDate > endDate)
            return BadRequest("Start date must be earlier than end date.");

        var statistics = await _statisticsRepository.GetPunctualityStatisticsAsync(startDate, endDate, routeNumber);
        return Ok(statistics);
    }
}
