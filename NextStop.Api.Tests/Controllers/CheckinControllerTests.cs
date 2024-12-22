using Microsoft.AspNetCore.Mvc;
using NextStop.Controllers;
using NextStop.Dtos;
using NextStop.Infrastructure.Persistence.Repositories;
using NSubstitute;

namespace NextStop.Api.Tests.Controllers
{
    [TestFixture]
    public class CheckinControllerTests
    {
        private CheckinController _controller;
        private ICheckinRepository _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = Substitute.For<ICheckinRepository>();
            _controller = new CheckinController(_mockRepository);
        }

        [Test]
        public async Task AddCheckin_InvalidTimestamp_ReturnsBadRequest()
        {
            // Arrange
            var invalidCheckinDto = new CheckinDto
            {
                RouteId = 1,
                StopId = 1,
                Timestamp = DateTime.UtcNow.AddHours(-2) // More than 1 hour ago
            };

            // Act
            var result = await _controller.AddCheckin(invalidCheckinDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [Test]
        public async Task AddCheckin_ValidTimestamp_ReturnsCreatedAtAction()
        {
            // Arrange
            var validCheckinDto = new CheckinDto
            {
                RouteId = 1,
                StopId = 1,
                Timestamp = DateTime.UtcNow // Current timestamp
            };

            var mockCheckinId = 1;
            _mockRepository.AddCheckinAsync(validCheckinDto.RouteId, validCheckinDto.StopId, validCheckinDto.Timestamp)
                .Returns(mockCheckinId);

            // Act
            var result = await _controller.AddCheckin(validCheckinDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(201, createdAtActionResult.StatusCode);
            Assert.AreEqual(mockCheckinId, createdAtActionResult.RouteValues["id"]);
        }
    }
}
