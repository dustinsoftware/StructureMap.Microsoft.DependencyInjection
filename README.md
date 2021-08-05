# .NET Core 3.1 Structuremap Integration

Available on Nuget here: https://www.nuget.org/packages/StructureMap.Microsoft.DependencyInjection.Forked/

This was forked from [this underlying library](https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection) due to a bug around Disposing objects.

## Integration guide

Program.cs:
```cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new StructureMapContainerBuilderFactory())
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

Startup.cs:
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
}

public void ConfigureContainer(Container builder)
{
    builder.Configure(config =>
    {
        // Your services here
        config.AddRegistry(new MyRegistry());
    });
}
```

The registry:
```cs
public class MyRegistry : Registry
{
    public MyRegistry()
    {
        For<Something>().Singleton().Use<Something>();
    }
}
```

StructureMapContainerBuilderFactory.cs
```cs
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
```
