using System.Collections.Immutable;

namespace Sequence.Simulation
{
    public interface ISimulationStore
    {
        Task<IImmutableList<GameId>> GetSimulationsAsync(
            PlayerHandle player,
            CancellationToken cancellationToken);

        Task<GameId> SaveNewSimulationAsync(
            NewSimulation newSimulation,
            CancellationToken cancellationToken);
    }
}
