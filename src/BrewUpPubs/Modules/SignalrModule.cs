using BrewUpPubs.Modules.Pubs.Hubs;

namespace BrewUpPubs.Modules;

public class SignalrModule : IModule
{
    public bool IsEnabled => true;
    public int Order => 0;

    public IServiceCollection RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

        return builder.Services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<PubsHub>("/hubs/production");

        return endpoints;
    }
}