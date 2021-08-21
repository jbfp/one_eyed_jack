using Sequence.CreateGame;
using Sequence.GetGameList;
using Sequence.Test.Postgres;
using Xunit;

namespace Sequence.Test.GetGameList
{
    [Trait("Category", "Integration")]
    [Collection(PostgresDockerContainerCollection.Name)]
    public class PostgresGameListProviderTest : PostgresTestBase
    {
        public PostgresGameListProviderTest(PostgresDockerContainerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CanGetGameList()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);
            var opponents = new[] { Player2 };
            var sut = new PostgresGameListProvider(db);

            // When:
            var gameList = await sut.GetGamesForPlayerAsync(Player1, CancellationToken.None);

            // Then:
            Assert.NotNull(gameList);
            var gameListItem = Assert.Single(gameList.Games);
            Assert.Equal(gameId, gameListItem.GameId);
            Assert.Equal(Player1, gameListItem.CurrentPlayer);
            Assert.Equal(opponents, gameListItem.Opponents);
        }

        [Fact]
        public async Task GameList_NextPlayerIdIsNullWhenGameIsOver()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);

            await AddEventAsync(db, gameId, new GameEvent
            {
                ByPlayerId = new PlayerId(1),
                CardUsed = new Card(DeckNo.One, Suit.Spades, Rank.Ace),
                Chip = Team.Red,
                Coord = new Coord(4, 2),
                Index = 1,
            });

            var sut = new PostgresGameListProvider(db);

            // When:
            var gameList = await sut.GetGamesForPlayerAsync(Player1, CancellationToken.None);

            // Then:
            var gameListItem = Assert.Single(gameList.Games);
            Assert.Null(gameListItem.CurrentPlayer);
        }

        [Fact]
        public async Task CannotGetGamesAsBot()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var player = new PlayerHandle("Super Bot");
            _ = await CreateGameAsync(db, new NewPlayer(player, PlayerType.Bot));

            var sut = new PostgresGameListProvider(db);

            // When:
            var gameList = await sut.GetGamesForPlayerAsync(player, CancellationToken.None);

            // Then:
            Assert.Empty(gameList.Games);
        }
    }
}
