using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace StructureMap
{
    public sealed class StructureMapServiceProviderIsService : IServiceProviderIsService
    {
        private readonly IContainer _container;

        public StructureMapServiceProviderIsService(IContainer container) => _container = container;

        // https://github.com/dotnet/dotnet/blob/d25d3482f4c3c509977cc4d959c80cb78b6ad9c8/src/runtime/src/libraries/Microsoft.Extensions.DependencyInjection/src/ServiceLookup/CallSiteFactory.cs#L778-L803
        public bool IsService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            // Querying for an open generic should return false (they aren't resolvable)
            if (serviceType.IsGenericTypeDefinition)
            {
                return false;
            }

            if (_container.Model.HasDefaultImplementationFor(serviceType))
            {
                return true;
            }


            if (serviceType.IsConstructedGenericType && serviceType.GetGenericTypeDefinition() is Type genericDefinition)
            {
                // We special case IEnumerable since it isn't explicitly registered in the container
                // yet we can manifest instances of it when requested.
                return genericDefinition == typeof(IEnumerable<>) ||
                    _container.Model.HasDefaultImplementationFor(genericDefinition);
            }

            return false;
        }
    }
}
