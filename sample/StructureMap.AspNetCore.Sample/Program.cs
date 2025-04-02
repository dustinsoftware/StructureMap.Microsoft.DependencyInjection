using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StructureMap;
using StructureMap.AspNetCore.Sample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});

builder.Host
    .UseServiceProviderFactory(new StructureMapContainerBuilderFactory())
    .ConfigureContainer<Container>(container =>
    {
        container.Configure(config =>
        {
            config.AddRegistry(new SampleRegistry());
        });
    });

var app = builder.Build();
app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
#pragma warning restore ASP0014

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.Run();
