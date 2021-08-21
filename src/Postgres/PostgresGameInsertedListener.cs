using Dapper;
using Npgsql;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace Sequence.Postgres
{
    public sealed class PostgresGameInsertedListener : BackgroundService, IObservable<GameId>
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;
        private readonly ILogger<PostgresGameInsertedListener> _logger;
        private readonly Subject<GameId> _subject = new();

        public PostgresGameInsertedListener(
            NpgsqlConnectionFactory connectionFactory,
            ILogger<PostgresGameInsertedListener> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<GameId> observer)
        {
            return _subject.Subscribe(observer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var connection = await _connectionFactory.CreateAndOpenAsync(stoppingToken);

                var notifications = Observable
                    .FromEvent<NotificationEventHandler, NpgsqlNotificationEventArgs>(
                        action => new NotificationEventHandler((_, args) => action(args)),
                        handler => connection.Notification += handler,
                        handler => connection.Notification -= handler)
                    .Select(args => args.Payload)
                    .Select(payload => JsonSerializer.Deserialize<GameRow>(payload))
                    .Select(row => row!.game_id);

                notifications.Subscribe(_subject);

                var command = new CommandDefinition(
                    commandText: "LISTEN game_inserted;",
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Starting game_inserted listener");

                await connection.ExecuteAsync(command);

                _logger.LogInformation("Started game_inserted listener");

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await connection.WaitAsync(stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred whilst listening for game events");
                }

                _subject.OnCompleted();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception. Restarting loop");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Give system a chance to recover
                await ExecuteAsync(stoppingToken);
            }
        }

#pragma warning disable CS0649
        private sealed class GameRow
        {
            public GameId game_id = null!;
        }
#pragma warning restore CS0649
    }
}
