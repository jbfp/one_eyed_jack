using System.Collections.Immutable;

namespace Sequence.GetGameList
{
    public sealed record GameList(IImmutableList<GameListItem> Games);
}
