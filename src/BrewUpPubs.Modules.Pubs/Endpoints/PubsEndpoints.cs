using BrewUpPubs.Modules.Pubs.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BrewUpPubs.Modules.Pubs.Endpoints;

public static class PubsEndpoints
{
    //[Authorize(Policy = "Tempo")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "data_analysis")]
    public static async Task<IResult> HandleGetBeers(IBeerService beerService)
    {
        var beersResult = await beerService.GetBeersAsync();

        return beersResult.Match(Results.Ok,
            _ => Results.BadRequest("An error occurred processing your request"));
    }
}