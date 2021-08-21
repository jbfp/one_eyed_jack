using System.Collections.Immutable;

namespace Sequence.Simulation
{
    public sealed record SimulationParams(IImmutableList<Bot> Players, BoardType BoardType, PlayerHandle CreatedBy, bool RandomFirstPlayer, Seed Seed, int WinCondition);
}
