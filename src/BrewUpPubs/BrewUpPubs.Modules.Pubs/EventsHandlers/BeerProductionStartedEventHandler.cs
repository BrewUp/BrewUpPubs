using BrewUpPubs.Modules.Pubs.Abstracts;
using BrewUpPubs.Modules.Pubs.Shared.Events;
using Microsoft.Extensions.Logging;

namespace BrewUpPubs.Modules.Pubs.EventsHandlers;

public sealed class BeerProductionStartedEventHandler : PubsDomainEventHandler<BeerProductionStarted>
{
    private readonly IBeerService _beerService;

    public BeerProductionStartedEventHandler(ILoggerFactory loggerFactory,
        IBeerService beerService) : base(loggerFactory)
    {
        _beerService = beerService;
    }

    public override async Task HandleAsync(BeerProductionStarted @event, CancellationToken cancellationToken = new())
    {
        try
        {
            await _beerService.CreateBeerAsync(@event.BeerId, @event.BeerType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"An error occurred processing event {@event.MessageId}. Message: {ex.Message}");
            throw;
        }
    }
}