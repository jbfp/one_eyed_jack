using Microsoft.AspNetCore.Mvc.Testing;
using Sequence.Test.Postgres;
using System.Net;
using Xunit;

namespace Sequence.Test.GetGame
{
    public sealed class GetGameIntegrationTest : IntegrationTestBase
    {
        private const string GameBasePath = "/games";
        private const string BoardBasePath = "/boards";

        public GetGameIntegrationTest(
            PostgresDockerContainerFixture fixture,
            WebApplicationFactory<Startup> factory) : base(fixture, factory)
        {
        }

        [Theory]
        [InlineData("32ceba86-ff6a-46cc-8b79-838dbec3ccb7")]
        [InlineData("9f97262e-284f-49a3-87ab-22eb0e5b6289")]
        [InlineData("b460f19a-7261-4bf8-8b0b-1097df0bf611")]
        public async Task RoutesAreProtected(string gameId)
        {
            var requestUri = $"{GameBasePath}/{gameId}";

            using var response = await UnauthorizedClient.GetAsync(requestUri);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetGameReturnsNotFound()
        {
            var requestUri = $"{GameBasePath}/idontexistforsure";

            using var response = await AuthorizedClient.GetAsync(requestUri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetGameReturnsOk()
        {
            var gamePath = await CreateGameAsync();

            using var response = await AuthorizedClient.GetAsync(gamePath);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(BoardBasePath)]
        [InlineData(BoardBasePath + "/sequence")]
        public async Task BoardRoutesAreProtected(string route)
        {
            using var response = await UnauthorizedClient.GetAsync(route);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("sequence")]
        [InlineData("seQuenCe")]
        [InlineData("OneEyedJACk")]
        [InlineData("oneeyedjack")]
        public async Task BoardTypeFromKey(string key)
        {
            using var response = await GetAsync(BoardBasePath, key);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("i dont exist")]
        [InlineData("foiwejfowiefjowiejf")]
        [InlineData("390r23r9#####@")]
        public async Task InvalidBoardTypeBadRequest(string key)
        {
            using var response = await GetAsync(BoardBasePath, key);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task BoardTypeCaching()
        {
            using var response = await GetAsync(BoardBasePath, "sequence");

            Assert.NotNull(response.Headers.CacheControl);
            Assert.True(response.Headers.CacheControl!.Public);
            Assert.NotNull(response.Headers.CacheControl!.MaxAge);
            Assert.Equal(response.Headers.CacheControl!.MaxAge!.Value, TimeSpan.FromDays(4 * 30.4375));
        }

        private async Task<HttpResponseMessage> GetAsync(string basePath, string? path = null)
        {
            if (path != null)
            {
                path = $"/{Uri.EscapeDataString(path)}";
            }

            return await AuthorizedClient.GetAsync($"{basePath}{path}");
        }
    }
}
