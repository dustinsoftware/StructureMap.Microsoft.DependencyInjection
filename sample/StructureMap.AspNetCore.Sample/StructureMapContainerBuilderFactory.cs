using System;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace StructureMap.AspNetCore.Sample;

public class StructureMapContainerBuilderFactory : IServiceProviderFactory<Container>
{
    private IServiceCollection _services;

    public Container CreateBuilder(IServiceCollection services)
    {
        _services = services;
        return new Container();
    }

    public IServiceProvider CreateServiceProvider(Container builder)
    {
        builder.Configure(config =>
        {
            config.Populate(_services);
        });

        return builder.GetInstance<IServiceProvider>();
    }
}
