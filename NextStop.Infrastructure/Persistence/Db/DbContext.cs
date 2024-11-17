using System.Data;

namespace NextStop.Infrastructure.Persistence.Db
{
    public class DbContext : IDisposable
    {
        private readonly IDbConnection _connection;

        public DbContext(DbConnectionFactory factory)
        {
            _connection = factory.CreateConnection();
        }

        public IDbConnection Connection => _connection;

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}