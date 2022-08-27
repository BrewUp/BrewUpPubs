using BrewUpPubs.Modules.Pubs.Abstracts;
using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using BrewUpPubs.Modules.Pubs.Shared.Dtos;
using BrewUpPubs.ReadModel.Abstracts;
using BrewUpPubs.ReadModel.Models;
using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;

namespace BrewUpPubs.Modules.Pubs.Concretes;

public sealed class BeerService : PubsBaseService, IBeerService
{
    public BeerService(IFunctionalPersister functionalPersister,
        ILoggerFactory loggerFactory) : base(functionalPersister, loggerFactory)
    {
    }

    public async Task<Either<Exception, string>> CreateBeerAsync(BeerId beerId, BeerType beerType)
    {
        var beer = Beer.CreateBeer(beerId, beerType);
        return await FunctionalPersister.InsertAsync(beer);
    }

    public async Task<Either<Exception, string>> UpdateBeerQuantityAsync(BeerId beerId, Quantity quantity)
    {
        await FunctionalPersister.GetByIdAsync<Beer>(beerId.ToString())
            .BindAsync(async beer =>
            {
                beer.UpdateQuantity(quantity);
                var updateProperties = new Dictionary<string, object>
                {
                    { "Quantity", beer.Quantity }
                };

                return await FunctionalPersister.UpdateOneAsync<Beer>(beer.Id, updateProperties);
            });

        return Left(new Exception("An error occurred while updating Beer"));
    }

    public async Task<Either<Exception, IEnumerable<BeerJson>>> GetBeersAsync()
    {
        return await FunctionalPersister.FindAsync<Beer>()
            .BindAsync(async beers => await MapToJson(beers));
    }

    private static Task<Either<Exception,IEnumerable<BeerJson>>> MapToJson(Beer[] beers)
    {
        if (beers == null)
            return Task.FromResult<Either<Exception, IEnumerable<BeerJson>>>(Right(Enumerable.Empty<BeerJson>()));

        var beersArray = beers.ToArray();
        var beerList = beersArray.Select(b => b.ToJson());

        return Task.FromResult<Either<Exception, IEnumerable<BeerJson>>>(Right(beerList));
    }
}