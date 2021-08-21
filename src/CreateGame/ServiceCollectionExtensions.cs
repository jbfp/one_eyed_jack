namespace Sequence.CreateGame
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCreateGameFeature(this IServiceCollection services) => services
            .AddScoped<CreateGameHandler>()
            .AddSingleton<IRandomFactory, SystemRandomFactory>()
            .AddScoped<IGameStore, PostgresGameStore>();
    }
}
