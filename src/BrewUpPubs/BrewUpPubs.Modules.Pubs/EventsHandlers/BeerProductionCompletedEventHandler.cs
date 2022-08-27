using BrewUpPubs.Modules.Pubs.Abstracts;
using BrewUpPubs.Modules.Pubs.Shared.Events;
using BrewUpPubs.Shared.Concretes;
using Microsoft.Extensions.Logging;

namespace BrewUpPubs.Modules.Pubs.EventsHandlers;

public sealed class BeerProductionCompletedEventHandler : PubsDomainEventHandler<BeerProductionCompleted>
{
    private readonly IBeerService _beerService;

    public BeerProductionCompletedEventHandler(ILoggerFactory loggerFactory,
        IBeerService beerService) : base(loggerFactory)
    {
        _beerService = beerService;
    }

    public override async Task HandleAsync(BeerProductionCompleted @event, CancellationToken cancellationToken = new())
    {
        try
        {
            await _beerService.UpdateBeerQuantityAsync(@event.BeerId, @event.Quantity);
        }
        catch (Exception ex)
        {
            Logger.LogError(CommonServices.GetDefaultErrorTrace(ex));
            throw;
        }
    }
}