using System.Collections.Immutable;

namespace Sequence.Simulation
{
    public sealed record NewSimulation(BoardType BoardType, PlayerHandle CreatedBy, int FirstPlayerIndex, IImmutableList<Bot> Players, Seed Seed, int WinCondition);
}
