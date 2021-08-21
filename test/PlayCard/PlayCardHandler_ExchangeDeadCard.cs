
using Moq;
using Sequence.PlayCard;
using System.Collections.Immutable;
using Xunit;

namespace Sequence.Test.PlayCard
{
    public sealed class PlayCardHandler_ExchangeDeadCard
    {
        private static readonly Player _player1 = new(
            new PlayerId(1),
            new PlayerHandle("player 1"),
            PlayerType.User
        );

        private static readonly Player _player2 = new(
            new PlayerId(2),
            new PlayerHandle("player 2"),
            PlayerType.User
        );

        private readonly Mock<IGameStateProvider> _provider = new();
        private readonly Mock<IGameEventStore> _store = new();
        private readonly Mock<IRealTimeContext> _realTime = new();

        private readonly PlayCardHandler _sut;

        private readonly GameId _gameId = GameIdGenerator.Generate();
        private readonly Card _deadCard = new(DeckNo.Two, Suit.Spades, Rank.Ten);

        private readonly GameState _game;

        public PlayCardHandler_ExchangeDeadCard()
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
                     ImmutableList.Create(
                    _player1,
                    _player2),
                _player1.Id,
                new Seed(42),
                BoardType.OneEyedJack,
                2
            )).Apply(new GameEvent
            {
                ByPlayerId = _player1.Id,
                CardUsed = new Card(DeckNo.One, Suit.Clubs, Rank.Jack),
                Chip = Team.Red,
                Coord = new Coord(1, 9),
                Index = 1,
                NextPlayerId = _player1.Id,
            }).Apply(new GameEvent
            {
                ByPlayerId = _player1.Id,
                CardUsed = new Card(DeckNo.Two, Suit.Clubs, Rank.Jack),
                Chip = Team.Red,
                Coord = new Coord(8, 0),
                Index = 2,
                NextPlayerId = _player1.Id,
            });
        }

        [Fact]
        public async Task ThrowsWhenCanceled()
        {
            var cancellationToken = new CancellationToken(canceled: true);

            await Assert.ThrowsAsync<OperationCanceledException>(
                testCode: () => _sut.ExchangeDeadCardAsync(_gameId, _player1.Id, _deadCard, cancellationToken)
            );
        }

        [Fact]
        public async Task ThrowsIfGameDoesNotExist()
        {
            await Assert.ThrowsAsync<GameNotFoundException>(
                () => _sut.ExchangeDeadCardAsync(_gameId, _player1.Id, _deadCard, CancellationToken.None)
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
            await _sut.ExchangeDeadCardAsync(_gameId, _player1.Id, _deadCard, CancellationToken.None);

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
            await _sut.ExchangeDeadCardAsync(_gameId, _player1.Id, _deadCard, CancellationToken.None);

            // Then:
            _store.Verify();
        }
    }
}
