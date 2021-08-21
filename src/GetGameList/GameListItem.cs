using System.Collections.Immutable;

namespace Sequence.GetGameList
{
    public sealed record GameListItem(GameId GameId, PlayerHandle CurrentPlayer, IImmutableList<PlayerHandle> Opponents, DateTimeOffset? LastMoveAt);
}
