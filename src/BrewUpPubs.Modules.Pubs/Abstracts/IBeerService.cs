using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using BrewUpPubs.Modules.Pubs.Shared.Dtos;
using LanguageExt;

namespace BrewUpPubs.Modules.Pubs.Abstracts;

public interface IBeerService
{
    Task<Either<Exception, string>> CreateBeerAsync(BeerId beerId, BeerType beerType);
    Task<Either<Exception, string>> UpdateBeerQuantityAsync(BeerId beerId, Quantity quantity);

    Task<Either<Exception, IEnumerable<BeerJson>>> GetBeersAsync();
}