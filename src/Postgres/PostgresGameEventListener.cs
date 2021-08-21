using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace Sequence.Postgres
{
    public sealed class PostgresGameEventListener : BackgroundService, IObservable<(GameId, GameEvent)>
    {
        private readonly NpgsqlConnectionFactory _connectionFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<PostgresGameEventListener> _logger;
        private readonly Subject<(GameId, GameEvent)> _subject = new();

        public PostgresGameEventListener(
            NpgsqlConnectionFactory connectionFactory,
            IOptions<JsonOptions> jsonOptions,
            ILogger<PostgresGameEventListener> logger)
        {
            _connectionFactory = connectionFactory;
            _jsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
            _logger = logger;
        }

        public IDisposable Subscribe(IObserver<(GameId, GameEvent)> observer)
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
                    .Select(payload => JsonSerializer.Deserialize<GameEventRow>(payload, _jsonSerializerOptions))
                    .Select(MapToReturnType);

                notifications.Subscribe(_subject);

                var command = new CommandDefinition(
                    commandText: "LISTEN game_event_inserted;",
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Starting game_event_inserted listener");

                await connection.ExecuteAsync(command);

                _logger.LogInformation("Started game_event_inserted listener");

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
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Give system a chance to recover.
                await ExecuteAsync(stoppingToken);
            }
        }

        private static (GameId, GameEvent) MapToReturnType(GameEventRow? row)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var gameId = row.surrogate_game_id;
            var gameEvent = row.ToGameEvent();
            return (gameId, gameEvent);
        }
    }
}
