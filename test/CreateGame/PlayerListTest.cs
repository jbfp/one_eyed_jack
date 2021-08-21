using Sequence.CreateGame;
using Xunit;

namespace Sequence.Test.CreateGame
{
    public sealed class PlayerListTest
    {
        [Fact]
        public void Constructor_FailsIfAnyPlayersAreSame()
        {
            var player = TestPlayer.Get;

            Assert.Throws<ArgumentException>(() =>
                new PlayerList(randomFirstPlayer: false, player, player));
        }

        [Fact]
        public void Constructor_MultipleBotsOfSameTypeIsAllowed()
        {
            var bot = new NewPlayer(
                new PlayerHandle("Resistance is futile"),
                PlayerType.Bot);

            Assert.NotNull(new PlayerList(randomFirstPlayer: false, bot, bot));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(7)]
        [InlineData(100)]
        [InlineData(42)]
        public void Constructor_OnlyCertainGamesSizesAllowed(int size)
        {
            var players = Enumerable
                .Range(0, size)
                .Select(_ => TestPlayer.Get)
                .ToArray();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => new PlayerList(randomFirstPlayer: false, players)
            );
        }
    }
}
