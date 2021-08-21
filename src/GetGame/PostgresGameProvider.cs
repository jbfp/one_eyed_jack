namespace Sequence.GetGame
{
    public sealed class PostgresGameProvider : IGameProvider
    {
        private readonly Postgres.PostgresGameProvider _gameProvider;

        public PostgresGameProvider(Postgres.PostgresGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public async Task<Game?> GetGameByIdAsync(
            GameId gameId,
            CancellationToken cancellationToken)
        {
            var tuple = await _gameProvider.GetById(gameId, cancellationToken);

            if (tuple == null)
            {
                return null;
            }

            var initialState = new GameState(tuple.Value.Item1);
            var gameEvents = tuple.Value.Item2;
            return new Game(initialState, gameEvents);
        }
    }
}
