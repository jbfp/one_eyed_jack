namespace Sequence.GetGame
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameViewFeature(this IServiceCollection services) =>
            services.AddSingleton<IGameProvider, PostgresGameProvider>();
    }
}
