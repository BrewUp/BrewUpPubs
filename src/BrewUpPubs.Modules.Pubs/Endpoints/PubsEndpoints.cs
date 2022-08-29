using BrewUpPubs.Modules.Pubs.Abstracts;
using Microsoft.AspNetCore.Http;

namespace BrewUpPubs.Modules.Pubs.Endpoints;

public static class PubsEndpoints
{
    public static async Task<IResult> HandleGetBeers(IBeerService beerService)
    {
        var beersResult = await beerService.GetBeersAsync();

        return beersResult.Match(Results.Ok,
            _ => Results.BadRequest("An error occurred processing your request"));
    }
}