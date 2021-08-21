using Dapper;
using Sequence.Postgres;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sequence.Bots
{
    public sealed class PostgresListener : BackgroundService, IObservable<BotTask>
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;
        private readonly IObservable<GameId> _newGameObservable;
        private readonly IObservable<(GameId, GameEvent)> _newGameEventObservable;
        private readonly ILogger<PostgresListener> _logger;

        private readonly ReplaySubject<BotTask> _subject = new(window: TimeSpan.FromSeconds(30));

        public PostgresListener(
            NpgsqlConnectionFactory connectionFactory,
            IObservable<GameId> newGameObservable,
            IObservable<(GameId, GameEvent)> newGameEventObservable,
            ILogger<PostgresListener> logger)
        {
            _connectionFactory = connectionFactory;
            _newGameObservable = newGameObservable;
            _newGameEventObservable = newGameEventObservable;
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<BotTask> observer)
        {
            return _subject.Subscribe(observer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var newGameEvents = _newGameObservable
                    .SelectMany(GetLatestBotTaskForGame)
                    .Replay(window: TimeSpan.FromSeconds(30));

                newGameEvents.Connect();

                var notifications = _newGameEventObservable
                    .Select(tuple => tuple.Item1)
                    .SelectMany(GetLatestBotTaskForGame)
                    .Replay(window: TimeSpan.FromSeconds(30));

                notifications.Connect();

                foreach (var botTask in await GetInitialBotTasksAsync(stoppingToken))
                {
                    _subject.OnNext(botTask);
                }

                newGameEvents.Subscribe(_subject);
                notifications.Subscribe(_subject);

                try
                {
                    await Observable
                        .Concat(newGameEvents, notifications)
                        .Select(_ => Unit.Default)
                        .DefaultIfEmpty(Unit.Default)
                        .RunAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }

                _subject.OnCompleted();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception. Restarting loop");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Give system a chance to recover.
                await ExecuteAsync(stoppingToken);
            }
        }

        private async Task<IImmutableList<BotTask>> GetInitialBotTasksAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting initial bot tasks");

            using var connection = await _connectionFactory.CreateAndOpenAsync(cancellationToken);

            const string sql = @"
                    SELECT
                      DISTINCT(g.game_id)
                    , gp.id
                    , gp.player_id
                    FROM public.game_player AS gp

                    INNER JOIN public.game AS g
                    ON g.id = gp.game_id

                    LEFT JOIN public.game_event AS ge
                    ON g.id = ge.game_id

                    WHERE ge.game_id IS NULL
                    AND gp.player_type = 'bot'
                    AND g.first_player_id = gp.id

                    UNION

                    SELECT
                      DISTINCT(g.game_id)
                    , gp.id
                    , gp.player_id
                    FROM public.game_player AS gp

                    INNER JOIN (
                        SELECT DISTINCT ON (game_id) * FROM
                        public.game_event
                        ORDER BY game_id ASC, idx DESC
                    ) AS ge
                    ON ge.game_id = gp.game_id

                    INNER JOIN public.game AS g
                    ON g.id = gp.game_id

                    WHERE gp.player_type = 'bot'
                    AND ge.next_player_id = gp.id;";

            var command = new CommandDefinition(
                commandText: sql,
                cancellationToken: cancellationToken);

            var rows = (await connection.QueryAsync<LatestBotTaskForGame>(command)).AsList();

            _logger.LogInformation("Got {NumTasks} initial bot tasks", rows.Count);

            return rows
                .Select(row => new BotTask(row.game_id, new Player(row.id, row.player_id, PlayerType.Bot)))
                .ToImmutableList();
        }

        private IObservable<BotTask> GetLatestBotTaskForGame(GameId gameId)
        {
            return Observable.Using(_connectionFactory.CreateAndOpenAsync, async (connection, cancellationToken) =>
            {
                _logger.LogInformation("Getting latest bot task for game {GameId}", gameId);

                const string sql = @"
                    SELECT
                      DISTINCT(g.game_id)
                    , gp.id
                    , gp.player_id
                    FROM public.game_player AS gp

                    INNER JOIN public.game AS g
                    ON g.id = gp.game_id

                    LEFT JOIN public.game_event AS ge
                    ON g.id = ge.game_id

                    WHERE ge.game_id IS NULL
                    AND gp.player_type = 'bot'
                    AND g.first_player_id = gp.id
                    AND g.game_id = @gameId

                    UNION

                    SELECT
                      g.game_id
                    , gp.id
                    , gp.player_id
                    FROM public.game_player AS gp

                    INNER JOIN (
                        SELECT DISTINCT ON (game_id) * FROM
                        public.game_event
                        ORDER BY game_id ASC, idx DESC
                    ) AS ge
                    ON ge.game_id = gp.game_id

                    INNER JOIN public.game AS g
                    ON g.id = gp.game_id

                    WHERE gp.player_type = 'bot'
                    AND ge.next_player_id = gp.id
                    AND g.game_id = @gameId;";

                var command = new CommandDefinition(
                    commandText: sql,
                    parameters: new { gameId },
                    cancellationToken: cancellationToken);

                var row = await connection.QuerySingleOrDefaultAsync<LatestBotTaskForGame>(command);

                if (row == null)
                {
                    _logger.LogWarning("Found no bot tasks for game {GameId}", gameId);
                    return Observable.Empty<BotTask>();
                }

                _logger.LogInformation(
                    "Found bot task for player {playerId} in game {gameId}",
                    row.player_id, row.game_id);

                var player = new Player(row.id, row.player_id, PlayerType.Bot);
                var botTask = new BotTask(row.game_id, player);
                return Observable.Return(botTask);
            });
        }


#pragma warning disable CS0649
        private sealed class LatestBotTaskForGame
        {
            public GameId game_id = null!;
            public PlayerId id = null!;
            public PlayerHandle player_id = null!;
        }
#pragma warning restore CS0649
    }
}
