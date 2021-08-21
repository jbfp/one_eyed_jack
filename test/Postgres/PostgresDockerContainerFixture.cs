using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using Sequence.Postgres;
using Xunit;

namespace Sequence.Test.Postgres
{
    [CollectionDefinition(Name)]
    public sealed class PostgresDockerContainerCollection
        : ICollectionFixture<PostgresDockerContainerFixture>
    {
        public const string Name = "Postgres Docker Container Collection";
    }

    public sealed class PostgresDockerContainerFixture : IDisposable
    {
        public const int Port = 15432;

        private readonly string _containerId;
        private readonly string _connectionString =
            $"Host=localhost; Port={Port}; Database=postgres; User ID=postgres;";

        public PostgresDockerContainerFixture()
        {
            _containerId = $"docker run --rm -d -e \"POSTGRES_HOST_AUTH_METHOD=trust\" -p {Port}:5432 postgres -N 100".Bash().Trim();

            // Try connecting until database is ready.
            while (true)
            {
                using var connection = new NpgsqlConnection(_connectionString);

                try
                {
                    connection.Open();
                    break;
                }
                catch (NpgsqlException)
                {
                    // Try again.
                    Thread.Sleep(500);
                }
            }
        }

        public async Task<NpgsqlConnectionFactory> CreateDatabaseAsync(
            CancellationToken cancellationToken)
        {
            // Create test database.
            var databaseSuffix = Guid.NewGuid();
            var databaseName = $"sequence_{databaseSuffix:N}";

            string connectionString;

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"CREATE DATABASE {databaseName};");
                connection.ChangeDatabase(databaseName);
                connectionString = connection.ConnectionString;
            }

            // Apply migrations.
            var postgresOptions = new PostgresOptions { ConnectionString = connectionString };
            var options = Options.Create(postgresOptions);
            var logger = NullLogger<PostgresMigrations>.Instance;
            var migrations = new PostgresMigrations(options, logger);
            await migrations.UpgradeDatabaseAsync(cancellationToken);

            return new NpgsqlConnectionFactory(options);
        }

        public void Dispose()
        {
            $"docker stop {_containerId}".Bash();
        }
    }
}
