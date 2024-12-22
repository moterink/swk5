using Microsoft.AspNetCore.Mvc;
using NextStop.Controllers;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;
using NSubstitute;

namespace NextStop.Api.Tests.Controllers
{
    [TestFixture]
    public class HolidaysControllerTests
    {
        private HolidaysController _controller;
        private IHolidayRepository _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = Substitute.For<IHolidayRepository>();
            _controller = new HolidaysController(_mockRepository);
        }

        [Test]
        public async Task GetHolidays_ReturnsOkWithHolidays()
        {
            // Arrange
            var mockHolidays = new List<Holiday>
            {
                new Holiday { Id = 1, Name = "New Year", StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 1, 1) },
                new Holiday { Id = 2, Name = "Christmas", StartDate = new DateTime(2024, 12, 24), EndDate = new DateTime(2024, 12, 25) }
            };
            _mockRepository.GetAllAsync().Returns(mockHolidays);

            // Act
            var result = await _controller.GetHolidays();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(mockHolidays, okResult.Value);
        }

        [Test]
        public async Task AddHoliday_ValidHoliday_ReturnsCreatedAtAction()
        {
            // Arrange
            var newHoliday = new Holiday { Id = 3, Name = "Labor Day", StartDate = new DateTime(2024, 5, 1), EndDate = new DateTime(2024, 5, 2), };
            _mockRepository.AddAsync(newHoliday).Returns(newHoliday.Id);

            // Act
            var result = await _controller.AddHoliday(newHoliday);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newHoliday.Id, createdResult.RouteValues["id"]);
            Assert.AreEqual(newHoliday, createdResult.Value);
        }

        [Test]
        public async Task AddHoliday_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");

            var invalidHoliday = new Holiday { Id = 4, StartDate = new DateTime(2024, 7, 4), EndDate = new DateTime(2024, 7, 4), };

            // Act
            var result = await _controller.AddHoliday(invalidHoliday);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateHoliday_ValidId_ReturnsNoContent()
        {
            // Arrange
            var existingHoliday = new Holiday { Id = 1, Name = "Easter", StartDate = new DateTime(2024, 4, 1), EndDate = new DateTime(2024, 5, 2), };
            _mockRepository.UpdateAsync(existingHoliday).Returns(1);

            // Act
            var result = await _controller.UpdateHoliday(existingHoliday.Id, existingHoliday);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateHoliday_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var holiday = new Holiday { Id = 1, Name = "Easter", StartDate = new DateTime(2024, 4, 1), EndDate = new DateTime(2024, 5, 2), };

            // Act
            var result = await _controller.UpdateHoliday(2, holiday);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("ID mismatch", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateHoliday_HolidayNotFound_ReturnsNotFound()
        {
            // Arrange
            var holiday = new Holiday { Id = 1, Name = "Easter", StartDate = new DateTime(2024, 4, 1), EndDate = new DateTime(2024, 5, 2) };
            _mockRepository.UpdateAsync(holiday).Returns(0);

            // Act
            var result = await _controller.UpdateHoliday(holiday.Id, holiday);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteHoliday_ValidId_ReturnsNoContent()
        {
            // Arrange
            var holidayId = 1;
            _mockRepository.DeleteAsync(holidayId).Returns(1);

            // Act
            var result = await _controller.DeleteHoliday(holidayId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteHoliday_HolidayNotFound_ReturnsNotFound()
        {
            // Arrange
            var holidayId = 1;
            _mockRepository.DeleteAsync(holidayId).Returns(0);

            // Act
            var result = await _controller.DeleteHoliday(holidayId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
