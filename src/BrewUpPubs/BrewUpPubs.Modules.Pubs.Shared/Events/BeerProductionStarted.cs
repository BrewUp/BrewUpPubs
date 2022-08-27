using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs.Shared.Events
{
    public class BeerProductionStarted : DomainEvent
    {
        public readonly BeerId BeerId;
        public readonly BeerType BeerType;

        public readonly BatchId BatchId;
        public readonly BatchNumber BatchNumber;

        public readonly Quantity Quantity;
        public readonly ProductionStartTime ProductionStartTime;

        public BeerProductionStarted(BeerId aggregateId, BeerType beerType, BatchId batchId, BatchNumber batchNumber,
            Quantity quantity, ProductionStartTime productionStartTime) : base(aggregateId)
        {
            BeerId = aggregateId;
            BeerType = beerType;

            BatchId = batchId;
            BatchNumber = batchNumber;

            Quantity = quantity;
            ProductionStartTime = productionStartTime;
        }
    }
}
