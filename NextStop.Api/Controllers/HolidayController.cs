using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;

namespace NextStop.Controllers
{
    [ApiController]
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    public class HolidaysController : ControllerBase
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidaysController(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetHolidays()
        {
            var holidays = await _holidayRepository.GetAllAsync();
            return Ok(holidays);
        }

        [HttpPost]
        public async Task<IActionResult> AddHoliday([FromBody] Holiday holiday)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _holidayRepository.AddAsync(holiday);
            return CreatedAtAction(nameof(GetHolidays), new { id }, holiday);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoliday(int id, [FromBody] Holiday holiday)
        {
            if (id != holiday.Id) return BadRequest("ID mismatch");

            var rowsAffected = await _holidayRepository.UpdateAsync(holiday);
            if (rowsAffected == 0) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var rowsAffected = await _holidayRepository.DeleteAsync(id);
            if (rowsAffected == 0) return NotFound();

            return NoContent();
        }
    }
}