using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;

namespace NextStop.Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class RouteRepositoryTests
    {
        private NpgsqlDataSource _mockDataSource;
        private RouteRepository _routeRepository;

        [SetUp]
        public void SetUp()
        {
            _mockDataSource = Substitute.For<NpgsqlDataSource>();
            _routeRepository = new RouteRepository(_mockDataSource);
        }

        [Test]
        public async Task GetTimetableAsync_ShouldReturnRoutes_WhenRoutesExist()
        {
            // Arrange
            var mockConnection = Substitute.For<NpgsqlConnection>();
            var mockCommand = Substitute.For<NpgsqlCommand>();
            var mockReader = Substitute.For<IDataReader>();

            _mockDataSource.OpenConnectionAsync().Returns(mockConnection);
            mockConnection.CreateCommand().Returns(mockCommand);

            mockCommand.ExecuteReaderAsync().Returns(Task.FromResult(mockReader));

            // Simulate the database returning one valid route
            mockReader.ReadAsync().Returns(Task.FromResult(true), Task.FromResult(false)); // First call returns true, then false
            mockReader.GetInt32(0).Returns(1); // Route ID
            mockReader.GetInt32(1).Returns(1); // Start Stop ID
            mockReader.GetInt32(2).Returns(2); // End Stop ID
            mockReader.GetDateTime(3).Returns(DateTime.Parse("2024-12-22T10:00:00")); // Departure Time
            mockReader.GetDateTime(4).Returns(DateTime.Parse("2024-12-22T10:30:00")); // Arrival Time

            // Act
            var result = await _routeRepository.GetTimetableAsync(1, 2, DateTime.Parse("2024-12-22T09:00:00"), 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var route = result[0];
            Assert.AreEqual(1, route.RouteId);
            Assert.AreEqual(1, route.StartStopId);
            Assert.AreEqual(2, route.EndStopId);
            Assert.AreEqual(DateTime.Parse("2024-12-22T10:00:00"), route.DepartureTime);
            Assert.AreEqual(DateTime.Parse("2024-12-22T10:30:00"), route.ArrivalTime);
        }

        [Test]
        public async Task GetTimetableAsync_ShouldReturnEmpty_WhenNoRoutesExist()
        {
            // Arrange
            var mockConnection = Substitute.For<NpgsqlConnection>();
            var mockCommand = Substitute.For<NpgsqlCommand>();
            var mockReader = Substitute.For<IDataReader>();

            _mockDataSource.OpenConnectionAsync().Returns(mockConnection);
            mockConnection.CreateCommand().Returns(mockCommand);

            mockCommand.ExecuteReaderAsync().Returns(Task.FromResult(mockReader));

            // Simulate no records returned
            mockReader.ReadAsync().Returns(Task.FromResult(false));

            // Act
            var result = await _routeRepository.GetTimetableAsync(1, 2, DateTime.Parse("2024-12-22T09:00:00"), 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetTimetableAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            _mockDataSource.OpenConnectionAsync().Returns<Task<NpgsqlConnection>>(x => throw new Exception("Database error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() =>
                _routeRepository.GetTimetableAsync(1, 2, DateTime.Parse("2024-12-22T09:00:00"), 1));
        }
    }
}
