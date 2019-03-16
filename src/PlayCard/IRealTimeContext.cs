using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sequence.PlayCard
{
    public interface IRealTimeContext
    {
        Task SendGameUpdatesAsync(GameId gameId, IEnumerable<GameUpdated> updates);
        Task SendGameUpdatesAsync(PlayerId playerId, IEnumerable<GameUpdated> updates);
    }
}
