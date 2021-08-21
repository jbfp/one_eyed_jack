using Moq;
using Sequence.CreateGame;
using Xunit;

namespace Sequence.Test.CreateGame
{
    public sealed class CreateGameHandlerTest
    {
        private static readonly PlayerList _twoPlayers = new(
            randomFirstPlayer: false,
            TestPlayer.Get,
            TestPlayer.Get);

        private static readonly BoardType _boardType = BoardType.OneEyedJack;

        private static readonly int _numSequencesToWin = 2;

        private readonly Mock<IRandomFactory> _randomFactory = new();
        private readonly Mock<IGameStore> _store = new();
        private readonly CreateGameHandler _sut;

        public CreateGameHandlerTest()
        {
            _randomFactory
                .Setup(s => s.Create())
                .Returns(new Random(42));

            _sut = new CreateGameHandler(_randomFactory.Object, _store.Object);
        }

        [Fact]
        public async Task CreateGameAsync_InvalidBoardType()
        {
            var boardType = (BoardType)1337;

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                paramName: "boardType",
                testCode: () => _sut.CreateGameAsync(_twoPlayers, boardType, _numSequencesToWin, CancellationToken.None)
            );
        }

        [Fact]
        public async Task CreateGameAsync_InvalidWinCondition()
        {
            var numSequencesToWin = 42;

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                paramName: "numSequencesToWin",
                testCode: () => _sut.CreateGameAsync(_twoPlayers, _boardType, numSequencesToWin, CancellationToken.None)
            );
        }

        [Theory]
        [InlineData(-60)]
        [InlineData(42)]
        [InlineData(100)]
        public async Task CreateGameAsync_GetsRandomFromFactory(int value)
        {
            // Given:
            var expected = new RandomStub(value);

            _randomFactory
                .Setup(s => s.Create())
                .Returns(expected)
                .Verifiable();

            // When:
            await _sut.CreateGameAsync(_twoPlayers, _boardType, _numSequencesToWin, CancellationToken.None);

            // Then:
            _randomFactory.VerifyAll();
        }

        [Theory]
        [InlineData("291bcf7e-45a2-4c5a-b16f-af8f9fa1b2df")]
        [InlineData("6a91eb4b-423a-41aa-8b5f-f5587260a4ed")]
        [InlineData("8dc05e8a-b8e1-4062-ab58-3c7c67436ecb")]
        public async Task CreateGameAsync_ReturnsGameIdFromStore(string gameIdStr)
        {
            var expected = new GameId(Guid.Parse(gameIdStr));

            _store
                .Setup(s => s.PersistNewGameAsync(It.IsAny<NewGame>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _sut.CreateGameAsync(_twoPlayers, _boardType, _numSequencesToWin, CancellationToken.None);

            Assert.Equal(expected, actual);
        }
    }
}
