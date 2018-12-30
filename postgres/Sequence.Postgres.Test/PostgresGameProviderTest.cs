using Sequence.Core;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sequence.Postgres.Test
{
    [Collection(PostgresDockerContainerCollection.Name)]
    public sealed class PostgresGameProviderTest : PostgresTestBase
    {
        public PostgresGameProviderTest(PostgresDockerContainerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task CanGetGame()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);
            var sut = new PostgresGameProvider(db);

            // Then:
            var game = await sut.GetGameByIdAsync(gameId, CancellationToken.None);

            // Then:
            Assert.NotNull(game);
        }

        [Fact]
        public async Task CanGetGameWithOneEvent()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);

            await AddEventAsync(db, gameId, new GameEvent
            {
                ByPlayerId = new PlayerId(1),
                CardDrawn = null,
                CardUsed = new Card(DeckNo.Two, Suit.Diamonds, Rank.King),
                Chip = Team.Green,
                Coord = new Coord(4, 2),
                Index = 2,
                NextPlayerId = new PlayerId(2),
            });

            var sut = new PostgresGameProvider(db);

            // Then:
            var game = await sut.GetGameByIdAsync(gameId, CancellationToken.None);

            // Then:
            Assert.NotNull(game);
        }

        [Fact]
        public async Task CanGetGameWithOneEventWithCardDrawn()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);

            await AddEventAsync(db, gameId, new GameEvent
            {
                ByPlayerId = new PlayerId(1),
                CardDrawn = new Card(DeckNo.One, Suit.Spades, Rank.Five),
                CardUsed = new Card(DeckNo.Two, Suit.Diamonds, Rank.King),
                Chip = Team.Green,
                Coord = new Coord(4, 2),
                Index = 2,
                NextPlayerId = new PlayerId(2),
            });

            var sut = new PostgresGameProvider(db);

            // Then:
            var game = await sut.GetGameByIdAsync(gameId, CancellationToken.None);

            // Then:
            Assert.NotNull(game);
        }

        [Fact]
        public async Task CanGetGameWithOneEventWithSequence()
        {
            // Given:
            var db = await CreateDatabaseAsync();
            var gameId = await CreateGameAsync(db);

            await AddEventAsync(db, gameId, new GameEvent
            {
                ByPlayerId = new PlayerId(1),
                CardDrawn = new Card(DeckNo.One, Suit.Spades, Rank.Five),
                CardUsed = new Card(DeckNo.Two, Suit.Diamonds, Rank.King),
                Chip = Team.Green,
                Coord = new Coord(4, 2),
                Index = 2,
                NextPlayerId = new PlayerId(2),
                Sequence = new Seq(Team.Blue, ImmutableList.Create(
                    new Coord(4, 2),
                    new Coord(5, 2),
                    new Coord(6, 2),
                    new Coord(7, 2),
                    new Coord(8, 2)
                )),
            });

            var sut = new PostgresGameProvider(db);

            // Then:
            var game = await sut.GetGameByIdAsync(gameId, CancellationToken.None);

            // Then:
            Assert.NotNull(game);
        }
    }
}
