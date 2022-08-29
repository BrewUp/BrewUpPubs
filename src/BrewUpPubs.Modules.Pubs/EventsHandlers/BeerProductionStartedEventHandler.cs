using BrewUpPubs.Modules.Pubs.Abstracts;
using BrewUpPubs.Modules.Pubs.Hubs;
using BrewUpPubs.Modules.Pubs.Shared.Events;
using Microsoft.Extensions.Logging;

namespace BrewUpPubs.Modules.Pubs.EventsHandlers;

public sealed class BeerProductionStartedEventHandler : PubsDomainEventHandler<BeerProductionStarted>
{
    private readonly IBeerService _beerService;
    private readonly PubsHub _pubsHub;

    public BeerProductionStartedEventHandler(ILoggerFactory loggerFactory,
        IBeerService beerService, PubsHub pubsHub) : base(loggerFactory)
    {
        _beerService = beerService;
        _pubsHub = pubsHub;
    }

    public override async Task HandleAsync(BeerProductionStarted @event, CancellationToken cancellationToken = new())
    {
        try
        {
            await _beerService.CreateBeerAsync(@event.BeerId, @event.BeerType);
            await _pubsHub.ProductionOrderStartedUpdatedAsync(@event.BatchId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"An error occurred processing event {@event.MessageId}. Message: {ex.Message}");
            throw;
        }
    }
}