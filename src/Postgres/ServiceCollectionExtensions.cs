namespace Sequence.Postgres
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgres(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var configSection = configuration.GetSection("Postgres");

            return services
                .AddTransient<IGameStateProvider, PostgresGameStateProvider>()
                .Configure<PostgresOptions>(configSection)
                .AddSingleton<NpgsqlConnectionFactory>()
                .AddSingleton<PostgresMigrations>()
                .AddSingleton<PostgresGameProvider>()
                .AddSingleton<PostgresGameInsertedListener>()
                .AddSingleton<IHostedService>(
                    sp => sp.GetRequiredService<PostgresGameInsertedListener>())
                .AddSingleton<IObservable<GameId>>(
                    sp => sp.GetRequiredService<PostgresGameInsertedListener>())
                .AddSingleton<PostgresGameEventListener>()
                .AddSingleton<IHostedService, PostgresGameEventListener>(
                    sp => sp.GetRequiredService<PostgresGameEventListener>())
                .AddSingleton<IObservable<(GameId, GameEvent)>>(
                    sp => sp.GetRequiredService<PostgresGameEventListener>())
                .AddHealthChecks()
                .AddNpgSql(configSection["ConnectionString"])
                .Services;
        }
    }
}
