﻿using BrewUpPubs.Modules.Pubs.Shared.CustomTypes;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs.Shared.Events;

public class BeerProductionCompleted : DomainEvent
{
    public readonly BatchId BatchId;
    public readonly BatchNumber BatchNumber;

    public readonly BeerId BeerId;

    public readonly Quantity Quantity;
    public readonly ProductionCompleteTime ProductionCompleteTime;

    public BeerProductionCompleted(BatchId aggregateId, BatchNumber batchNumber, BeerId beerId, Quantity quantity,
        ProductionCompleteTime productionCompleteTime) : base(aggregateId)
    {
        BatchId = aggregateId;
        BatchNumber = batchNumber;

        BeerId = beerId;

        ProductionCompleteTime = productionCompleteTime;
        Quantity = quantity;
    }
}