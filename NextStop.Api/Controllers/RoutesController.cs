using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStop.Dtos;
using NextStop.Infrastructure.Persistence.Repositories;
using NextStop.Infrastructure.Persistence.Entities;

namespace NextStop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteRepository _routeRepository;

    public RoutesController(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        var routes = await _routeRepository.GetAllAsync();
        return Ok(routes);
    }

    [HttpPost]
    [Authorize(Roles = "operator")]
    public async Task<IActionResult> AddRoute([FromBody] RouteDto routeDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var route = new NextStop.Infrastructure.Persistence.Entities.Route
        {
            Number = routeDto.Number,
            ValidFrom = routeDto.ValidFrom,
            ValidTo = routeDto.ValidTo,
            DaysOfOperation = routeDto.DaysOfOperation
        };

        route.Stops = routeDto.Stops.Select((s, index) => new RouteStop
        {
            StopId = s.StopId,
            SequenceNumber = index + 1,
            ScheduledDepartureTime = s.ScheduledDepartureTime
        }).ToList();

        var routeId = await _routeRepository.AddAsync(route);
        return CreatedAtAction(nameof(AddRoute), new { id = routeId }, null);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "operator")]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] NextStop.Infrastructure.Persistence.Entities.Route route)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _routeRepository.UpdateAsync(route);
        if (result == 0) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "operator")]
    public async Task<IActionResult> DeleteRoute(int id)
    {
        var result = await _routeRepository.DeleteAsync(id);
        if (result == 0) return NotFound();

        return NoContent();
    }
    
    [HttpGet("timetable")]
    public async Task<IActionResult> GetTimetable(
        [FromQuery] int startStopId,
        [FromQuery] int endStopId,
        [FromQuery] DateTime dateTime,
        [FromQuery] bool isArrivalTime = false,
        [FromQuery] int limit = 3)
    {
        if (startStopId <= 0 || endStopId <= 0)
            return BadRequest(new { message = "Start and end stop IDs must be valid." });

        if (startStopId == endStopId)
            return BadRequest(new { message = "Start and end stop IDs must not be the same." });
        
        var results = await _routeRepository.GetTimetableAsync(
            startStopId, endStopId, dateTime, isArrivalTime, limit);
    
        return Ok(results);
    }

}