namespace Sequence.Bots
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBotsFeature(this IServiceCollection services) => services
            .AddSingleton<BotTaskHandler>()
            .AddHostedService<BotTaskObserver>()
            .AddSingleton<PostgresListener>()
            .AddSingleton<IHostedService>(sp => sp.GetRequiredService<PostgresListener>())
            .AddSingleton<IObservable<BotTask>>(sp =>
                sp.GetRequiredService<PostgresListener>());
    }
}
