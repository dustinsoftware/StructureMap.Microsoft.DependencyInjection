using System;
using Microsoft.Extensions.DependencyInjection;

namespace StructureMap
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStructureMap(this IServiceCollection services)
        {
            return AddStructureMap(services, registry: null);
        }

        public static IServiceCollection AddStructureMap(this IServiceCollection services, Registry registry)
        {
            return services.AddSingleton<IServiceProviderFactory<Registry>>(new StructureMapServiceProviderFactory(registry));
        }

        /// <summary>
        /// Configures <paramref name="services"/> with <paramref name="configure"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The service configurator.</param>
        /// <returns>The result of <paramref name="configure"/>, which should be <paramref name="services"/>.</returns>
        public static IServiceCollection Configure(this IServiceCollection services, Func<IServiceCollection, IServiceCollection> configure) => configure(services);
    }
}