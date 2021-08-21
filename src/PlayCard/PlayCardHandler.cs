using System.Collections.Immutable;

namespace Sequence.PlayCard
{
    public sealed class PlayCardHandler
    {
        private readonly IGameStateProvider _provider;
        private readonly IGameEventStore _store;
        private readonly IRealTimeContext _realTime;

        public PlayCardHandler(
            IGameStateProvider provider,
            IGameEventStore store,
            IRealTimeContext realTime)
        {
            _provider = provider;
            _store = store;
            _realTime = realTime;
        }

        public async Task<IImmutableList<Move>> GetMovesForPlayerAsync(
            GameId gameId,
            PlayerHandle player,
            CancellationToken cancellationToken)
        {
            var state = await _provider.GetGameByIdAsync(gameId, cancellationToken);

            if (state == null)
            {
                throw new GameNotFoundException();
            }

            return Moves.GenerateMoves(state, player);
        }

        public async Task<IImmutableList<Move>> GetMovesForPlayerAsync(
            GameId gameId,
            PlayerId player,
            CancellationToken cancellationToken)
        {
            var state = await _provider.GetGameByIdAsync(gameId, cancellationToken);

            if (state == null)
            {
                throw new GameNotFoundException();
            }

            return Moves.GenerateMoves(state, player);
        }

        public Task<IEnumerable<GameUpdated>> PlayCardAsync(
            GameId gameId,
            PlayerHandle player,
            Card card,
            Coord coord,
            CancellationToken cancellationToken)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            return DoThing(
                gameId,
                state => Game.PlayCard(state, player, card, coord),
                cancellationToken);
        }

        public Task<IEnumerable<GameUpdated>> PlayCardAsync(
            GameId gameId,
            PlayerId player,
            Card card,
            Coord coord,
            CancellationToken cancellationToken)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            return DoThing(
                gameId,
                state => Game.PlayCard(state, player, card, coord),
                cancellationToken);
        }

        public Task<IEnumerable<GameUpdated>> ExchangeDeadCardAsync(
            GameId gameId,
            PlayerHandle player,
            Card deadCard,
            CancellationToken cancellationToken)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (deadCard == null)
            {
                throw new ArgumentNullException(nameof(deadCard));
            }

            return DoThing(
                gameId,
                state => Game.ExchangeDeadCard(state, player, deadCard),
                cancellationToken);
        }

        public Task<IEnumerable<GameUpdated>> ExchangeDeadCardAsync(
            GameId gameId,
            PlayerId player,
            Card deadCard,
            CancellationToken cancellationToken)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (deadCard == null)
            {
                throw new ArgumentNullException(nameof(deadCard));
            }

            return DoThing(
                gameId,
                state => Game.ExchangeDeadCard(state, player, deadCard),
                cancellationToken);
        }

        private async Task<IEnumerable<GameUpdated>> DoThing(
            GameId gameId,
            Func<GameState, GameEvent> doThing,
            CancellationToken cancellationToken)
        {
            if (gameId == null)
            {
                throw new ArgumentNullException(nameof(gameId));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var state = await _provider.GetGameByIdAsync(gameId, cancellationToken);

            if (state == null)
            {
                throw new GameNotFoundException();
            }

            var gameEvent = doThing(state);
            await _store.AddEventAsync(gameId, gameEvent, cancellationToken);
            var newState = new GetGame.Game(state, gameEvent);

#pragma warning disable CA2016
            _ = Task.Run(() =>
            {
                var broadcast = _realTime.SendGameUpdatesAsync(
                    gameId, newState.GenerateForPlayer((PlayerId?)null));

                var tasks = state
                    .PlayerIdByIdx
                    .AsParallel()
                    .Where(playerId => playerId != gameEvent.ByPlayerId)
                    .Select(playerId => (playerId, updates: newState.GenerateForPlayer(playerId)))
                    .Select(t => _realTime.SendGameUpdatesAsync(t.playerId, t.updates))
                    .Prepend(broadcast);

                return Task.WhenAll(tasks);
            });
#pragma warning restore CA2016

            return newState.GenerateForPlayer(gameEvent.ByPlayerId);
        }
    }
}
