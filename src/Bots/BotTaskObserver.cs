using System.Reactive;
using System.Reactive.Linq;

namespace Sequence.Bots
{
    public sealed class BotTaskObserver : BackgroundService
    {
        private readonly IObservable<BotTask> _observable;
        private readonly BotTaskHandler _handler;
        private readonly ILogger<BotTaskObserver> _logger;

        public BotTaskObserver(
            IObservable<BotTask> observable,
            BotTaskHandler handler,
            ILogger<BotTaskObserver> logger)
        {
            _observable = observable;
            _handler = handler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Subscribing to bot task observable");

            await _observable
                .SelectMany(botTask => Observable
                    .FromAsync(ct => _handler.HandleBotTaskAsync(botTask, ct)))
                .DefaultIfEmpty(Unit.Default)
                .RunAsync(stoppingToken);

            _logger.LogInformation("Bot task observable complete");
        }
    }
}
