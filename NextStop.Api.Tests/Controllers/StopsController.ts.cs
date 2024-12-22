using NSubstitute;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;
using NextStop.Controllers;

namespace NextStop.Api.Tests.Controllers
{
    [TestFixture]
    public class StopsControllerTests
    {
        private IStopRepository _mockRepository;
        private StopsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = Substitute.For<IStopRepository>();
            _controller = new StopsController(_mockRepository);
        }

        [Test]
        public async Task UpdateStop_ReturnsConflict_WhenShortNameExists()
        {
            // Arrange
            var stopToUpdate = new Stop
            {
                Id = 1,
                Name = "Updated Stop",
                ShortName = "DUPLICATE",
                Latitude = 10.123,
                Longitude = 20.456
            };

            _mockRepository.ExistsByShortNameAsync(stopToUpdate.ShortName, stopToUpdate.Id).Returns(true); // Simulate conflict

            // Act
            var result = await _controller.UpdateStop(stopToUpdate.Id, stopToUpdate);

            // Assert
            Assert.That(result, Is.TypeOf<ConflictObjectResult>());
            var conflictResult = (ConflictObjectResult)result;
            Assert.IsNotNull(conflictResult.Value);
            Assert.That(((dynamic)conflictResult.Value).message, Is.EqualTo("A stop with the same short_name already exists."));
        }

        [Test]
        public async Task UpdateStop_ReturnsNoContent_WhenUpdateSuccessful()
        {
            // Arrange
            var stopToUpdate = new Stop
            {
                Id = 1,
                Name = "Updated Stop",
                ShortName = "UNIQUE",
                Latitude = 10.123,
                Longitude = 20.456
            };

            _mockRepository.ExistsByShortNameAsync(stopToUpdate.ShortName, stopToUpdate.Id).Returns(false); // No conflict

            _mockRepository.UpdateAsync(stopToUpdate).Returns(1); // Simulate successful update

            // Act
            var result = await _controller.UpdateStop(stopToUpdate.Id, stopToUpdate);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateStop_ReturnsNotFound_WhenStopDoesNotExist()
        {
            // Arrange
            var stopToUpdate = new Stop
            {
                Id = 1,
                Name = "Non-Existent Stop",
                ShortName = "UNIQUE",
                Latitude = 10.123,
                Longitude = 20.456
            };

            _mockRepository.ExistsByShortNameAsync(stopToUpdate.ShortName, stopToUpdate.Id).Returns(false); // No conflict

            _mockRepository.UpdateAsync(stopToUpdate).Returns(0); // Simulate no rows affected

            // Act
            var result = await _controller.UpdateStop(stopToUpdate.Id, stopToUpdate);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateStop_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var stopToUpdate = new Stop
            {
                Id = 2, // ID mismatch
                Name = "Updated Stop",
                ShortName = "UNIQUE",
                Latitude = 10.123,
                Longitude = 20.456
            };

            // Act
            var result = await _controller.UpdateStop(1, stopToUpdate); // URL ID is 1, body ID is 2

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.That(badRequestResult.Value, Is.EqualTo("ID in the URL does not match the ID in the body."));
        }
    }
}
