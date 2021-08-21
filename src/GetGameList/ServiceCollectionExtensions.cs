namespace Sequence.GetGameList
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameListFeature(this IServiceCollection services) => services
            .AddScoped<GetGameListHandler>()
            .AddScoped<IGameListProvider, PostgresGameListProvider>();
    }
}
