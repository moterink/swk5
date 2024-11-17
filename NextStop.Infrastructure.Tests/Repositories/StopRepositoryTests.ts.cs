using Npgsql;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextStop.Infrastructure.Persistence.Db;
using NextStop.Infrastructure.Persistence.Entities;
using NextStop.Infrastructure.Persistence.Repositories;
using NSubstitute;

namespace NextStop.Tests.Repositories
{
    [TestFixture]
    public class StopRepositoryTests
    {
        private NpgsqlDataSource _mockDataSource;
        private NpgsqlConnection _mockConnection;
        private NpgsqlCommand _mockCommand;
        private NpgsqlDataReader _mockReader;
        private StopRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockDataSource = Substitute.For<NpgsqlDataSource>();
            _mockConnection = Substitute.For<NpgsqlConnection>();
            _mockCommand = Substitute.For<NpgsqlCommand>();
            _mockReader = Substitute.For<NpgsqlDataReader>();

            _mockConnection.CreateCommand().Returns(_mockCommand);

            _mockCommand.ExecuteReaderAsync().Returns(_mockReader);

            _repository = new StopRepository(_mockDataSource);
        }
        
        [TearDown]
        public void TearDown()
        {
            // Dispose all IDisposable mocks
            _mockDataSource?.Dispose();
            _mockReader?.Dispose();
            _mockCommand?.Dispose();
            _mockConnection?.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnListOfStops()
        {
            _mockReader.ReadAsync().Returns(
                true,
                true,
                false
            );

            _mockReader.GetInt32(0).Returns(1, 2);
            _mockReader.GetString(1).Returns("Stop 1", "Stop 2");
            _mockReader.GetString(2).Returns("S1", "S2");
            _mockReader.GetDouble(3).Returns(48.1, 48.2);
            _mockReader.GetDouble(4).Returns(16.3, 16.4);

            var stops = await _repository.GetAllAsync();

            Assert.That(stops, Is.Not.Null);
            Assert.That(stops, Has.Count.EqualTo(2));
            Assert.That(stops, Has.One.Matches<Stop>(s => s.Name == "Stop 1" && s.ShortName == "S1"));
            Assert.That(stops, Has.One.Matches<Stop>(s => s.Name == "Stop 2" && s.ShortName == "S2"));
        }

        [Test]
        public async Task AddAsync_ShouldReturnNewId()
        {
            // Mock the ExecuteScalarAsync result for the INSERT query
            _mockCommand.ExecuteScalarAsync().Returns(123);

            // Create a new Stop to add
            var stop = new Stop
            {
                Name = "Test Stop",
                ShortName = "TST",
                Latitude = 48.1,
                Longitude = 16.3
            };

            var newId = await _repository.AddAsync(stop);

            Assert.That(newId, Is.EqualTo(123));
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnAffectedRows()
        {
            _mockCommand.ExecuteNonQueryAsync().Returns(1);

            var stop = new Stop
            {
                Id = 1,
                Name = "Updated Stop",
                ShortName = "UST",
                Latitude = 48.2,
                Longitude = 16.4
            };

            var rowsAffected = await _repository.UpdateAsync(stop);

            Assert.That(rowsAffected, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnAffectedRows()
        {
            _mockCommand.ExecuteNonQueryAsync().Returns(1);

            var rowsAffected = await _repository.DeleteAsync(1);
            
            Assert.That(rowsAffected, Is.EqualTo(1));
        }
    }
}
