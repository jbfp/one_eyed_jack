using Microsoft.AspNetCore.SignalR;

namespace Sequence.RealTime
{
    public sealed class GameHub : Hub<IGameHubClient>
    {
        private readonly ILogger<GameHub> _logger;

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }

        public async Task Subscribe(string gameId)
        {
            _logger.LogInformation("User subscribing to {GameId}", gameId);
            var connectionId = Context.ConnectionId;
            var groupName = gameId.ToString();
            var cancellationToken = Context.ConnectionAborted;
            cancellationToken.Register(() => { });
            await Groups.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }

        public async Task Identify(int playerId)
        {
            _logger.LogInformation("Player with ID {PlayerId} has connected", playerId);
            var connectionId = Context.ConnectionId;
            var groupName = playerId.ToString();
            var cancellationToken = Context.ConnectionAborted;
            await Groups.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }
    }
}
