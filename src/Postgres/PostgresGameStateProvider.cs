namespace Sequence.Postgres
{
    public sealed class PostgresGameStateProvider : IGameStateProvider
    {
        private readonly PostgresGameProvider _gameProvider;

        public PostgresGameStateProvider(PostgresGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public async Task<GameState?> GetGameByIdAsync(
            GameId gameId,
            CancellationToken cancellationToken)
        {
            var tuple = await _gameProvider.GetById(gameId, cancellationToken);

            if (tuple is null)
            {
                return null;
            }

            return new GameState(tuple.Value.Item1, tuple.Value.Item2);
        }
    }
}