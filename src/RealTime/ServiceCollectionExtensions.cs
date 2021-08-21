using Sequence.PlayCard;

namespace Sequence.RealTime
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRealTimeFeature(this IServiceCollection services) => services
            .AddTransient<IRealTimeContext, GameHubRealTimeContext>();
    }
}
