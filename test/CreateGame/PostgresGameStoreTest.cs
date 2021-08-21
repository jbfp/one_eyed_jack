using Sequence.CreateGame;
using Sequence.Test.Postgres;
using Xunit;

namespace Sequence.Test.CreateGame
{
    [Trait("Category", "Integration")]
    [Collection(PostgresDockerContainerCollection.Name)]
    public sealed class PostgresGameStoreTest : PostgresTestBase
    {
        public PostgresGameStoreTest(PostgresDockerContainerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CanPersistNewGame()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var sut = new PostgresGameStore(db);
            var newGame = new NewGame(
                players: new PlayerList(
                    randomFirstPlayer: false,
                    new NewPlayer(Player1, PlayerType.User),
                    new NewPlayer(Player2, PlayerType.User)),
                firstPlayerIndex: 0,
                seed: new Seed(42),
                boardType: BoardType.OneEyedJack,
                numSequencesToWin: 2);

            // When:
            var gameId = await sut.PersistNewGameAsync(newGame, CancellationToken.None);

            // Then:
            Assert.NotNull(gameId);
        }

        [Fact]
        public async Task CanPersistNewGameWithMultipleIdenticalBots()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var sut = new PostgresGameStore(db);
            var botType = new PlayerHandle("Dalvik");
            var newGame = new NewGame(
                players: new PlayerList(
                    randomFirstPlayer: false,
                    new NewPlayer(botType, PlayerType.Bot),
                    new NewPlayer(botType, PlayerType.Bot)),
                firstPlayerIndex: 0,
                seed: new Seed(42),
                boardType: BoardType.Sequence,
                numSequencesToWin: 2);

            // When:
            var gameId = await sut.PersistNewGameAsync(newGame, CancellationToken.None);

            // Then:
            Assert.NotNull(gameId);
        }
    }
}
