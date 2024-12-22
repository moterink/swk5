using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStop.Infrastructure.Persistence.Repositories;
using NextStop.Dtos;

namespace NextStop.Controllers
{
    [ApiController]
    [Authorize(Roles = "bus")]
    [Route("api/[controller]")]
    public class CheckinController : ControllerBase
    {
        private readonly ICheckinRepository _checkinRepository;

        public CheckinController(ICheckinRepository checkinRepository)
        {
            _checkinRepository = checkinRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddCheckin([FromBody] CheckinDto checkinDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            if (checkinDto.Timestamp < oneHourAgo)
            {
                return BadRequest(new { Error = "Check-in timestamps cannot be more than 1 hour old." });
            }

            var checkinId = await _checkinRepository.AddCheckinAsync(
                checkinDto.RouteId,
                checkinDto.StopId,
                checkinDto.Timestamp
            );

            return CreatedAtAction(nameof(AddCheckin), new { id = checkinId }, null);
        }
    }
}