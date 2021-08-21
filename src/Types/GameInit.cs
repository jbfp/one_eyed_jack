using System.Collections.Immutable;

namespace Sequence
{
    public sealed record GameInit(IImmutableList<Player> Players, PlayerId FirstPlayerId, Seed Seed, BoardType BoardType, int NumberOfSequencesToWin);
}
