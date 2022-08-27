using BrewUpPubs.Modules.Pubs.Abstracts;
using Microsoft.AspNetCore.Http;

namespace BrewUpPubs.Modules.Pubs.Endpoints;

public static class PubsEndpoints
{
    public static async Task<IResult> HandleGetBeers(IBeerService beerService)
    {
        var beers = await beerService.GetBeersAsync();

        return Results.Ok(beers);
    }
}