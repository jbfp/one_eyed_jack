using Moq;
using Sequence.PlayCard;
using System.Collections.Immutable;
using Xunit;

namespace Sequence.Test.PlayCard
{
    public sealed class PlayCardHandlerTest
    {
        private readonly Mock<IGameStateProvider> _provider = new();
        private readonly Mock<IGameEventStore> _store = new();
        private readonly Mock<IRealTimeContext> _realTime = new();

        private readonly PlayCardHandler _sut;

        private readonly GameId _gameId = GameIdGenerator.Generate();
        private readonly PlayerHandle _player = new("dummy 1");
        private readonly Card _card = new(DeckNo.Two, Suit.Spades, Rank.Ten);
        private readonly Coord _coord = new(1, 9);

        private readonly GameState _game;

        public PlayCardHandlerTest()
        {
            _provider
                .Setup(p => p.GetGameByIdAsync(_gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((GameState?)null)
                .Verifiable();

            _store
                .Setup(s => s.AddEventAsync(_gameId, It.IsAny<GameEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _realTime
                .Setup(r => r.SendGameUpdatesAsync(It.IsAny<PlayerId>(), It.IsAny<IEnumerable<GameUpdated>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _sut = new PlayCardHandler(_provider.Object, _store.Object, _realTime.Object);

            _game = new GameState(
                new GameInit(
                    Players: ImmutableArray.Create(
                        new Player(
                            new PlayerId(1),
                            _player,
                            PlayerType.User
                        ),
                        new Player(
                            new PlayerId(2),
                            new PlayerHandle("dummy 2"),
                            PlayerType.User
                        )
                    ),
                    FirstPlayerId: new PlayerId(1),
                    Seed: new Seed(42),
                    BoardType: BoardType.OneEyedJack,
                    NumberOfSequencesToWin: 2
                )
            );
        }

        [Fact]
        public async Task ThrowsWhenCanceled()
        {
            var cancellationToken = new CancellationToken(canceled: true);

            await Assert.ThrowsAsync<OperationCanceledException>(
                testCode: () => _sut.PlayCardAsync(_gameId, _player, _card, _coord, cancellationToken)
            );
        }

        [Fact]
        public async Task ThrowsIfGameDoesNotExist()
        {
            await Assert.ThrowsAsync<GameNotFoundException>(
                () => _sut.PlayCardAsync(_gameId, _player, _card, _coord, CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetsGameFromProvider()
        {
            // Given:
            _provider
                .Setup(p => p.GetGameByIdAsync(_gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_game)
                .Verifiable();

            // When:
            await _sut.PlayCardAsync(_gameId, _player, _card, _coord, CancellationToken.None);

            // Then:
            _provider.Verify();
        }

        [Fact]
        public async Task SavesEvent()
        {
            // Given:
            _provider
                .Setup(p => p.GetGameByIdAsync(_gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_game);

            // When:
            await _sut.PlayCardAsync(_gameId, _player, _card, _coord, CancellationToken.None);

            // Then:
            _store.Verify();
        }

        [Fact]
        public async Task UpdatesRealTimeComms()
        {
            // Given:
            _provider
                .Setup(p => p.GetGameByIdAsync(_gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_game);

            // When:
            await _sut.PlayCardAsync(_gameId, _player, _card, _coord, CancellationToken.None);
            await Task.Delay(1000); // Updating comms happens on threadpool.

            // Then:
            _realTime.Verify();
        }
    }
}
