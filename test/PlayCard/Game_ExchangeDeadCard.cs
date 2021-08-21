using Sequence.PlayCard;
using System.Collections.Immutable;
using Xunit;

namespace Sequence.Test.PlayCard
{
    public sealed class Game_ExchangeDeadCard
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

        private static readonly PlayerHandle _playerDummy = new("dummy");
        private static readonly Card _cardDummy = new(DeckNo.Two, Suit.Spades, Rank.Ten);
        private static readonly Coord _expectedCoord = new(-1, -1);

        private static readonly GameState _sut = new(
            new GameInit(
                ImmutableList.Create(
                    _player1,
                    _player2),
                _player1.Id,
                new Seed(42),
                BoardType.OneEyedJack,
                2));

        [Fact]
        public void ThrowsIfPlayerIsNotInGame()
        {
            var ex = Assert.Throws<ExchangeDeadCardFailedException>(
                () => Game.ExchangeDeadCard(_sut, _playerDummy, _cardDummy)
            );

            Assert.Equal(ExchangeDeadCardError.PlayerIsNotInGame, ex.Error);
        }

        [Fact]
        public void ThrowsIfPlayerIsCurrentPlayer()
        {
            var ex = Assert.Throws<ExchangeDeadCardFailedException>(
                () => Game.ExchangeDeadCard(_sut, _player2.Handle, _cardDummy)
            );

            Assert.Equal(ExchangeDeadCardError.PlayerIsNotCurrentPlayer, ex.Error);
        }

        [Fact]
        public void ThrowsIfPlayerDoesNotHaveCard()
        {
            var card = new Card(DeckNo.Two, Suit.Spades, Rank.Ace);

            var ex = Assert.Throws<ExchangeDeadCardFailedException>(
                () => Game.ExchangeDeadCard(_sut, _player1.Handle, card)
            );

            Assert.Equal(ExchangeDeadCardError.PlayerDoesNotHaveCard, ex.Error);
        }

        [Fact]
        public void ThrowsIfCardIsNotDead()
        {
            var ex = Assert.Throws<ExchangeDeadCardFailedException>(
                () => Game.ExchangeDeadCard(_sut, _player1.Handle, _cardDummy)
            );

            Assert.Equal(ExchangeDeadCardError.CardIsNotDead, ex.Error);
        }

        [Fact]
        public void ThrowsIfPlayerHasAlreadyExchangedDeadCard()
        {
            var sut = _sut.Apply(new GameEvent
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
            }).Apply(new GameEvent
            {
                ByPlayerId = _player1.Id,
                CardDrawn = new Card(DeckNo.Two, Suit.Clubs, Rank.Ten),
                CardUsed = new Card(DeckNo.One, Suit.Diamonds, Rank.Queen),
                Coord = _expectedCoord,
                Index = 1,
                NextPlayerId = _player1.Id,
            });

            var ex = Assert.Throws<ExchangeDeadCardFailedException>(
                () => Game.ExchangeDeadCard(sut, _player1.Handle, _cardDummy)
            );

            Assert.Equal(ExchangeDeadCardError.PlayerHasAlreadyExchangedDeadCard, ex.Error);
        }

        [Fact]
        public void HappyPath()
        {
            // Given:
            var sut = _sut.Apply(new GameEvent
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

            var expected = new GameEvent
            {
                ByPlayerId = _player1.Id,
                CardDrawn = new Card(DeckNo.Two, Suit.Clubs, Rank.Ten),
                CardUsed = _cardDummy,
                Coord = _expectedCoord,
                Index = 3,
                NextPlayerId = _player1.Id,
            };

            // When:
            var actual = Game.ExchangeDeadCard(sut, _player1.Handle, _cardDummy);

            // Then:
            Assert.Equal(expected, actual, new GameEventEqualityComparer());
        }
    }
}
