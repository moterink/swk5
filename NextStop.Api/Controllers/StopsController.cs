using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;

namespace NextStop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StopsController : ControllerBase
    {
        private readonly IStopRepository _stopRepository;

        public StopsController(IStopRepository stopRepository)
        {
            _stopRepository = stopRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStops([FromQuery] string? query = null)
        {
            IEnumerable<Stop> stops;

            if (string.IsNullOrWhiteSpace(query))
            {
                stops = await _stopRepository.GetAllAsync();
            }
            else
            {
                stops = await _stopRepository.SearchAsync(query);
            }

            return Ok(stops);
        }

        [HttpPost]
        [Authorize(Roles = "operator")]
        public async Task<IActionResult> AddStop([FromBody] Stop stop)
        {
            // Check if the short_name already exists for a different stop
            var shortNameConflict = await _stopRepository.ExistsByShortNameAsync(stop.ShortName, 0);
            if (shortNameConflict)
            {
                return Conflict(new { message = "A stop with this short_name already exists." });
            }
            
            var id = await _stopRepository.AddAsync(stop);
            
            return CreatedAtAction(nameof(GetStopById), new { id }, stop);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "operator")]
        public async Task<IActionResult> UpdateStop(int id, [FromBody] Stop updatedStop)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null) return NotFound();
            
            // Check if the short_name already exists for a different stop
            var shortNameConflict = await _stopRepository.ExistsByShortNameAsync(updatedStop.ShortName, id);
            if (shortNameConflict)
            {
                return Conflict(new { message = "A stop with this short_name already exists." });
            }

            updatedStop.Id = id;
            await _stopRepository.UpdateAsync(updatedStop);

            return NoContent();
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStopById(int id)
        {
            var stop = await _stopRepository.GetByIdAsync(id);
            if (stop == null) return NotFound();
            return Ok(stop);
        }
        
        [HttpGet("{id}/departures")]
        public async Task<IActionResult> GetNextDepartures(int id, [FromQuery] DateTime dateTime, [FromQuery] int limit = 5)
        {
            var departures = await _stopRepository.GetNextDeparturesAsync(id, dateTime, limit);
            return Ok(departures);
        }
    }
}
