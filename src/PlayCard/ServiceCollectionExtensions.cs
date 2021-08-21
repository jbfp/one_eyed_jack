namespace Sequence.PlayCard
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPlayCardFeature(this IServiceCollection services) => services
            .AddSingleton<PlayCardHandler>()
            .AddSingleton<IGameEventStore, PostgresGameEventStore>();
    }
}
