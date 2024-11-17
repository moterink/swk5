using Npgsql;

namespace NextStop.Infrastructure.Persistence.Db
{
    public class DbDataSource
    {
        private readonly NpgsqlDataSource _dataSource;

        public DbDataSource(string connectionString)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            _dataSource = dataSourceBuilder.Build();
        }

        public NpgsqlDataSource GetDataSource() => _dataSource;
    }
}