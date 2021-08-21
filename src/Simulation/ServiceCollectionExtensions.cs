namespace Sequence.Simulation
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimulationFeature(this IServiceCollection services) => services
            .AddScoped<SimulationHandler>()
            .AddScoped<ISimulationStore, PostgresSimulationStore>();
    }
}
