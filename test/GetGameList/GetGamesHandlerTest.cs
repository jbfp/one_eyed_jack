using Moq;
using Sequence.GetGameList;
using System.Collections.Immutable;
using Xunit;

namespace Sequence.Test.GetGameList
{
    public sealed class GetGameListHandlerTest
    {
        private readonly Mock<IGameListProvider> _provider = new();
        private readonly GetGameListHandler _sut;

        public GetGameListHandlerTest()
        {
            _sut = new GetGameListHandler(_provider.Object);
        }

        [Fact]
        public async Task ThrowsWhenCanceled()
        {
            var playerId = new PlayerHandle("dummy");
            var cancellationToken = new CancellationToken(canceled: true);

            await Assert.ThrowsAsync<OperationCanceledException>(
                testCode: () => _sut.GetGamesForPlayerAsync(playerId, cancellationToken)
            );
        }

        [Theory]
        [InlineData("player 1")]
        [InlineData("42")]
        [InlineData("true")]
        public async Task GetsGamesFromProviderForPlayer(string player)
        {
            // Given:
            var playerId = new PlayerHandle(player);
            var expected = new GameList(ImmutableList<GameListItem>.Empty);

            _provider
                .Setup(p => p.GetGamesForPlayerAsync(playerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected)
                .Verifiable();

            // When:
            var actual = await _sut.GetGamesForPlayerAsync(playerId, CancellationToken.None);

            // Then:
            Assert.Equal(expected, actual);
            _provider.VerifyAll();
        }
    }
}
