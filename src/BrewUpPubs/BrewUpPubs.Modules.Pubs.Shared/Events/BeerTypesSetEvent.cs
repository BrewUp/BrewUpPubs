using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs.Shared.Events;

public class BeerTypesSetEvent : DomainEvent
{
    public BeerType BeerType { get; }

    public BeerTypesSetEvent(BeerId aggregateId, BeerType beerType) :
        base(aggregateId)
    {
        BeerType = beerType;
    }
}