namespace Sequence.CreateGame
{
    public sealed class CreateGameHandler
    {
        private readonly IRandomFactory _randomFactory;
        private readonly IGameStore _gameStore;

        public CreateGameHandler(IRandomFactory randomFactory, IGameStore gameStore)
        {
            _randomFactory = randomFactory;
            _gameStore = gameStore;
        }

        public async Task<GameId> CreateGameAsync(
            PlayerList players,
            BoardType boardType,
            int numSequencesToWin,
            CancellationToken cancellationToken)
        {
            var random = _randomFactory.Create();
            var seed = new Seed(random.Next());
            var firstPlayerIdx = players.RandomFirstPlayer ? random.Next(players.Players.Count) : 0;
            var newGame = new NewGame(players, firstPlayerIdx, seed, boardType, numSequencesToWin);
            return await _gameStore.PersistNewGameAsync(newGame, cancellationToken);
        }
    }
}
