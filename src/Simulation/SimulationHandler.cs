using System.Collections.Immutable;

namespace Sequence.Simulation
{
    public sealed class SimulationHandler
    {
        private readonly ISimulationStore _store;

        public SimulationHandler(ISimulationStore store)
        {
            _store = store;
        }

        public async Task<GameId> CreateSimulationAsync(
            SimulationParams parameters,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var players = parameters.Players;
            var random = parameters.Seed.ToRandom();
            var firstPlayerIdx = parameters.RandomFirstPlayer ? random.Next(players.Count) : 0;

            var newSimulation = new NewSimulation
            (
                BoardType: parameters.BoardType,
                CreatedBy: parameters.CreatedBy,
                FirstPlayerIndex: firstPlayerIdx,
                Players: players,
                Seed: parameters.Seed,
                WinCondition: parameters.WinCondition
            );

            return await _store.SaveNewSimulationAsync(newSimulation, cancellationToken);
        }

        public Task<IImmutableList<GameId>> GetSimulationsAsync(
            PlayerHandle player,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _store.GetSimulationsAsync(player, cancellationToken);
        }
    }
}
