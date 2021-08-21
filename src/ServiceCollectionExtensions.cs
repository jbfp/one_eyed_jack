using System.Runtime.CompilerServices;

namespace Sequence
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSequence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var @this = typeof(ServiceCollectionExtensions).GetMethod(nameof(AddSequence));

            var extensionMethods = typeof(ServiceCollectionExtensions)
                .Assembly
                .DefinedTypes
                .Where(type => type.CustomAttributes
                    .Any(attr => attr.AttributeType == typeof(ExtensionAttribute)))
                .SelectMany(type => type.DeclaredMethods)
                .Where(method =>
                    method != @this &&
                    method.IsPublic &&
                    method.ReturnType == typeof(IServiceCollection) &&
                    method.CustomAttributes.Any(attr =>
                        attr.AttributeType == typeof(ExtensionAttribute)));

            foreach (var method in extensionMethods)
            {
                var parameters = method.GetParameters();
                var arguments = new List<object>(parameters.Length);

                if (parameters.Length > 0)
                {
                    arguments.Add(services);
                }

                if (parameters.Length > 1)
                {
                    arguments.Add(configuration);
                }

                method.Invoke(null, arguments.ToArray());
            }

            return services;
        }
    }
}
