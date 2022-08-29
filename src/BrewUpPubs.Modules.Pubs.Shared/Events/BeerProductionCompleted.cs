using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs.Shared.Events;

public class BeerProductionCompleted : DomainEvent
{
    public readonly BeerId BeerId;

    public readonly BatchNumber BatchNumber;

    public readonly Quantity Quantity;
    public readonly ProductionCompleteTime ProductionCompleteTime;

    public BeerProductionCompleted(BeerId aggregateId, BatchNumber batchNumber, Quantity quantity,
        ProductionCompleteTime productionCompleteTime) : base(aggregateId)
    {
        BeerId = aggregateId;

        BatchNumber = batchNumber;

        Quantity = quantity;
        ProductionCompleteTime = productionCompleteTime;
    }
}