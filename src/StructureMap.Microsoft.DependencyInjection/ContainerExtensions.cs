﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public static class ContainerExtensions
    {
        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors)
        {
            container.Populate(descriptors, checkDuplicateCalls: false);
        }

        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="checkDuplicateCalls">Specifies whether duplicate calls to Populate should throw.</param>
        public static void Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors, bool checkDuplicateCalls)
        {
            container.Configure(config => config.Populate(descriptors, checkDuplicateCalls));
        }

        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="config">The configuration.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Populate(this ConfigurationExpression config, IEnumerable<ServiceDescriptor> descriptors)
        {
            config.Populate(descriptors, checkDuplicateCalls: false);
        }

        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="config">The configuration.</param>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="checkDuplicateCalls">Specifies whether duplicate calls to Populate should throw.</param>
        public static void Populate(this ConfigurationExpression config, IEnumerable<ServiceDescriptor> descriptors, bool checkDuplicateCalls)
        {
            ((Registry) config).Populate(descriptors, checkDuplicateCalls);
        }

        /// <inheritdoc cref="Populate(Registry, IEnumerable{ServiceDescriptor}, bool)"/>
        public static void Populate(this Registry registry, IEnumerable<ServiceDescriptor> descriptors)
        {
            registry.Populate(descriptors, checkDuplicateCalls: false);
        }

        /// <summary>
        /// Populates the registry using the specified service descriptors, plus:
        /// <list type="bullet">
        ///   <item><see cref="AspNetConstructorSelector"/></item>
        ///   <item><see cref="StructureMapServiceProvider"/> as per-container <see cref="IServiceProvider"/></item>
        ///   <item><see cref="StructureMapServiceScopeFactory"/> as per-container <see cref="IServiceScopeFactory"/></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="registry">The registry.</param>
        /// <param name="descriptors">The service descriptors.</param>
        /// <param name="checkDuplicateCalls">Specifies whether duplicate calls to Populate should throw.</param>
        public static void Populate(this Registry registry, IEnumerable<ServiceDescriptor> descriptors, bool checkDuplicateCalls)
        {
            if (checkDuplicateCalls)
            {
                // HACK: We insert this action in order to prevent Populate being called twice on the same container.
                registry.Configure(ThrowIfMarkerInterfaceIsRegistered);
            }

            registry.For<IMarkerInterface>();

            registry.Policies.ConstructorSelector<AspNetConstructorSelector>();

            registry.For<IServiceProvider>()
                .LifecycleIs(Lifecycles.Container)
                .Use<StructureMapServiceProvider>();

            registry.For<IServiceScopeFactory>()
                .LifecycleIs(Lifecycles.Container)
                .Use<StructureMapServiceScopeFactory>();

            registry.Register(descriptors);
        }

        private static void ThrowIfMarkerInterfaceIsRegistered(PluginGraph graph)
        {
            if (graph.HasFamily<IMarkerInterface>())
            {
                throw new InvalidOperationException("Populate should only be called once per container.");
            }
        }

        /// <summary>
        /// Configures <paramref name="registry"/> with <paramref name="configure"/>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Populate(Registry, IEnumerable{ServiceDescriptor})"/>, this method may be be called more than once per container.
        /// </remarks>
        /// <param name="registry">The registry.</param>
        /// <param name="configure">The service configurator.</param>
        public static void Configure(this IProfileRegistry registry, Func<IServiceCollection, IServiceCollection> configure) =>
            registry.Register(configure(new SimpleServiceCollection()));

        /// <summary>
        /// Registers the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Populate(Registry, IEnumerable{ServiceDescriptor})"/>, this method may be be called more than once per container.
        /// </remarks>
        /// <param name="registry">The registry.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Register(this IProfileRegistry registry, IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                registry.Register(descriptor);
            }
        }

        private static void Register(this IProfileRegistry registry, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                registry.For(descriptor.ServiceType)
                    .LifecycleIs(descriptor.Lifetime)
                    .Use(descriptor.ImplementationType);

                return;
            }

            if (descriptor.ImplementationFactory != null)
            {
                registry.For(descriptor.ServiceType)
                    .LifecycleIs(descriptor.Lifetime)
                    .Use(descriptor.CreateFactory());

                return;
            }

            registry.For(descriptor.ServiceType)
                .LifecycleIs(descriptor.Lifetime)
                .Use(descriptor.ImplementationInstance);
        }

        private static Expression<Func<IContext, object>> CreateFactory(this ServiceDescriptor descriptor)
        {
            return context => descriptor.ImplementationFactory(context.GetInstance<IServiceProvider>());
        }

        private interface IMarkerInterface { }

        // We don't depend on the package that declares ServiceCollection,
        // but even if we did this is slightly better because we don't need its IsReadOnly checks
        private class SimpleServiceCollection : List<ServiceDescriptor>, IServiceCollection
        {
        }
    }
}
