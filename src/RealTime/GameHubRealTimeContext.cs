using Microsoft.AspNetCore.SignalR;
using Sequence.PlayCard;

namespace Sequence.RealTime
{
    public sealed class GameHubRealTimeContext : IRealTimeContext
    {
        private readonly IHubContext<GameHub, IGameHubClient> _hub;

        public GameHubRealTimeContext(IHubContext<GameHub, IGameHubClient> hub)
        {
            _hub = hub;
        }

        public Task SendGameUpdatesAsync(GameId gameId, IEnumerable<GameUpdated> updates)
        {
            return SendUpdatesToGroupAsync(gameId.ToString(), updates);
        }

        public Task SendGameUpdatesAsync(PlayerId playerId, IEnumerable<GameUpdated> updates)
        {
            return SendUpdatesToGroupAsync(playerId.ToString(), updates);
        }

        private async Task SendUpdatesToGroupAsync(
            string groupName,
            IEnumerable<GameUpdated> updates)
        {
            var clients = _hub.Clients.Group(groupName);

            foreach (var update in updates)
            {
                await clients.UpdateGame(update);
            }
        }
    }
}
