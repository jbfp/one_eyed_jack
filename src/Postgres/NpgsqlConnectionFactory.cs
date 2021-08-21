using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Sequence.Postgres
{
    public sealed class NpgsqlConnectionFactory
    {
        static NpgsqlConnectionFactory()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<DeckNo>("deckno");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PlayerType>("player_type");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Rank>("rank");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Suit>("suit");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Team>("chip");

            NpgsqlConnection.GlobalTypeMapper.MapComposite<CardComposite>("card");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<CoordComposite>("coord");
            NpgsqlConnection.GlobalTypeMapper.MapComposite<SequenceComposite>("sequence");

            SqlMapper.AddTypeHandler(new GameIdTypeHandler());
            SqlMapper.AddTypeHandler(new PlayerHandleTypeHandler());
            SqlMapper.AddTypeHandler(new PlayerIdTypeHandler());
            SqlMapper.AddTypeHandler(new SeedTypeHandler());
        }

        private readonly IOptions<PostgresOptions> _options;

        public NpgsqlConnectionFactory(IOptions<PostgresOptions> options)
        {
            _options = options;
        }

        public string ConnectionString => _options.Value.ConnectionString;

        public async Task<NpgsqlConnection> CreateAndOpenAsync(CancellationToken cancellationToken)
        {
            var connectionString = _options.Value.ConnectionString;
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}
