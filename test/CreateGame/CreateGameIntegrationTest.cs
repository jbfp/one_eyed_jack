using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sequence.Test.Postgres;
using System.Net;
using Xunit;

namespace Sequence.Test.CreateGame
{
    [Trait("Category", "Integration")]
    public sealed class CreateGameIntegrationTest : IntegrationTestBase
    {
        private const string BotsBasePath = "/bots";
        private const string GamesBasePath = "/games";

        public CreateGameIntegrationTest(
            PostgresDockerContainerFixture fixture,
            WebApplicationFactory<Startup> factory) : base(fixture, factory)
        {
        }

        [Fact]
        public async Task GamesRouteIsProtected()
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin = 2,
                opponents = new[]
                {
                    new { name = "test", type = 1 },
                },
            };

            using var response = await UnauthorizedClient.PostAsJsonAsync(GamesBasePath, body);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateGameReturnsCreated()
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin = 2,
                opponents = new[] { new { name = "test", type = "User" } },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.NotNull(response.Content);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Theory]
        [InlineData("some bot type that does not exist")]
        [InlineData("jbfp")]
        [InlineData("hello world")]
        public async Task CreateGameReturnsBadRequestIfBotIsUnknown(string botType)
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin = 2,
                opponents = new[]
                {
                    new { name = botType, type = "Bot" },
                },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(-1)]
        [InlineData(42)]
        [InlineData(null)]
        public async Task CreateGameReturnsBadRequestIfBoardTypeIsUnknown(int? boardType)
        {
            var body = new
            {
                boardType,
                numSequencesToWin = 2,
                opponents = new[]
                {
                    new { name = "test", type = "User" },
                },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(-1)]
        [InlineData(42)]
        [InlineData(0)]
        public async Task CreateGameReturnsBadRequestIfWinConditionIsInvalid(int numSequencesToWin)
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin,
                opponents = new[]
                {
                    new { name = "test", type = "User" },
                },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGameReturnsBadRequestIfGameSizeIsInvalid()
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin = 1,
                opponents = new[]
                {
                    new { name = "fail1", type = "User" },
                    new { name = "fail2", type = "User" },
                    new { name = "fail3", type = "User" },
                    new { name = "fail4", type = "User" },
                },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeAnonymousType(json, new { error = "" });
            Assert.Equal("Game size is invalid.", obj.error);
        }

        [Fact]
        public async Task CreateGameReturnsBadRequestIfPlayer1AndPlayer2AreSame()
        {
            var body = new
            {
                boardType = 0,
                numSequencesToWin = 3,
                opponents = new[]
                {
                    new { name = DefaultUserName, type = "User" },
                },
            };

            using var response = await PostAsync(body);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeAnonymousType(json, new { error = "" });
            Assert.Equal("Duplicate players are not allowed.", obj.error);
        }

        [Fact]
        public async Task BotRouteIsProtected()
        {
            using var response = await UnauthorizedClient.GetAsync(BotsBasePath);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetBotsReturnsCorrectResult()
        {
            using var response = await AuthorizedClient.GetAsync(BotsBasePath);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.NotEmpty(json["botTypes"]);
        }

        private async Task<HttpResponseMessage> PostAsync<T>(T value)
        {
            return await AuthorizedClient.PostAsJsonAsync(GamesBasePath, value);
        }
    }
}
