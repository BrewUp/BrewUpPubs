using BrewUpPubs.Modules.Pubs.Endpoints;
using BrewUpPubs.Modules.Pubs.Shared;

namespace BrewUpPubs.Modules;

public class PubsModule : IModule
{
    private const string BaseEndpointUrl = "v1/pubs";

    public bool IsEnabled => true;
    public int Order => 99;

    public IServiceCollection RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddPubs();

        return builder.Services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{BaseEndpointUrl}/beers", PubsEndpoints.HandleGetBeers)
            .WithTags("Beers");

        return endpoints;
    }
}