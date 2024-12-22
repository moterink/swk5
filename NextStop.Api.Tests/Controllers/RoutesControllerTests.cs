using Microsoft.AspNetCore.Mvc;
using NextStop.Controllers;
using NextStop.Dtos;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace NextStop.Api.Tests.Controllers
{
    [TestFixture]
    public class RoutesControllerTests
    {
        private RoutesController _controller;
        private IRouteRepository _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = Substitute.For<IRouteRepository>();
            _controller = new RoutesController(_mockRepository);
        }

        [Test]
        public async Task GetAllRoutes_ReturnsOkWithRoutes()
        {
            // Arrange
            var mockRoutes = new List<Route>
            {
                new Route { Id = 1, Number = "101", ValidFrom = new DateOnly(2024, 1, 1), ValidTo = new DateOnly(2025, 1, 1) },
                new Route { Id = 2, Number = "102", ValidFrom = new DateOnly(2024, 1, 1), ValidTo = new DateOnly(2025, 1, 1) }
            };
            _mockRepository.GetAllAsync().Returns(mockRoutes);

            // Act
            var result = await _controller.GetAllRoutes();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(mockRoutes, okResult.Value);
        }

        [Test]
        public async Task AddRoute_ValidRoute_ReturnsCreatedAtAction()
        {
            // Arrange
            var routeDto = new RouteDto
            {
                Number = "101",
                ValidFrom = new DateOnly(2024, 1, 1),
                ValidTo = new DateOnly(2025, 1, 1),
                DaysOfOperation = "Mon-Fri",
                Stops = new List<RouteStopDto>
                {
                    new RouteStopDto { StopId = 1, ScheduledDepartureTime = new TimeOnly(9, 0) },
                    new RouteStopDto { StopId = 2, ScheduledDepartureTime = new TimeOnly(10, 0) }
                }
            };

            var newRouteId = 1;
            _mockRepository.AddAsync(Arg.Any<Route>()).Returns(newRouteId);

            // Act
            var result = await _controller.AddRoute(routeDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newRouteId, createdResult.RouteValues["id"]);
        }

        [Test]
        public async Task AddRoute_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Number", "Number is required");

            var routeDto = new RouteDto
            {
                ValidFrom = new DateOnly(2024, 1, 1),
                ValidTo = new DateOnly(2025, 1, 1),
                DaysOfOperation = "Mon-Fri",
                Stops = new List<RouteStopDto>
                {
                    new RouteStopDto { StopId = 1, ScheduledDepartureTime = new TimeOnly(9, 0) }
                }
            };

            // Act
            var result = await _controller.AddRoute(routeDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateRoute_ValidRoute_ReturnsNoContent()
        {
            // Arrange
            var route = new Route { Id = 1, Number = "101", ValidFrom = new DateOnly(2024, 1, 1), ValidTo = new DateOnly(2025, 1, 1) };
            _mockRepository.UpdateAsync(route).Returns(1);

            // Act
            var result = await _controller.UpdateRoute(route.Id, route);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateRoute_NotFound_ReturnsNotFound()
        {
            // Arrange
            var route = new Route { Id = 1, Number = "101", ValidFrom = new DateOnly(2024, 1, 1), ValidTo = new DateOnly(2025, 1, 1) };
            _mockRepository.UpdateAsync(route).Returns(0);

            // Act
            var result = await _controller.UpdateRoute(route.Id, route);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteRoute_ValidId_ReturnsNoContent()
        {
            // Arrange
            var routeId = 1;
            _mockRepository.DeleteAsync(routeId).Returns(1);

            // Act
            var result = await _controller.DeleteRoute(routeId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteRoute_NotFound_ReturnsNotFound()
        {
            // Arrange
            var routeId = 1;
            _mockRepository.DeleteAsync(routeId).Returns(0);

            // Act
            var result = await _controller.DeleteRoute(routeId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetTimetable_ValidParameters_ReturnsOkWithTimetable()
        {
            // Arrange
            var startStopId = 1;
            var endStopId = 2;
            var dateTime = new DateTime(2024, 1, 1);
            var departureTime = new TimeOnly(6, 0);
            var arrivalTime = new TimeOnly(7, 0, 0);
            var isArrivalTime = false;
            var limit = 3;

            var timetable = new List<TimetableResult> { new TimetableResult { RouteId = 1, StartStopId = 1, EndStopId = 2, DepartureTime = departureTime, ArrivalTime = arrivalTime } };
            _mockRepository.GetTimetableAsync(startStopId, endStopId, dateTime, isArrivalTime, limit).Returns(timetable);

            // Act
            var result = await _controller.GetTimetable(startStopId, endStopId, dateTime, isArrivalTime, limit);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(timetable, okResult.Value);
        }

        [Test]
        public async Task GetTimetable_InvalidParameters_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetTimetable(-1, -1, DateTime.UtcNow);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
