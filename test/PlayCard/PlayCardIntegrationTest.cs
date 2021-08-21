using Microsoft.AspNetCore.Mvc.Testing;
using Sequence.Test.Postgres;
using System.Net;
using Xunit;

namespace Sequence.Test.PlayCard
{
    [Trait("Category", "Integration")]
    public sealed class PlayCardIntegrationTest : IntegrationTestBase
    {
        private const string BasePath = "/games";

        public PlayCardIntegrationTest(
            PostgresDockerContainerFixture fixture,
            WebApplicationFactory<Startup> factory)
            : base(fixture, factory)
        {
        }

        [Theory]
        [InlineData("32ceba86-ff6a-46cc-8b79-838dbec3ccb7")]
        [InlineData("9f97262e-284f-49a3-87ab-22eb0e5b6289")]
        [InlineData("b460f19a-7261-4bf8-8b0b-1097df0bf611")]
        public async Task RoutesAreProtected(string gameId)
        {
            var requestUri = $"{BasePath}/{gameId}";
            var body = new { card = new { deckNo = 0, suit = 0, rank = 0 }, column = 0, row = 0 };

            using var response = await UnauthorizedClient.PostAsJsonAsync(requestUri, body);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task PlayReturnsNotFound()
        {
            var requestUri = $"{BasePath}/idontexistforsure";
            var body = new { card = new { deckNo = 0, suit = 0, rank = 0 }, column = 0, row = 0 };

            using var response = await AuthorizedClient.PostAsJsonAsync(requestUri, body);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
