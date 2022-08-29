using BrewUpPubs.Modules.Pubs.Shared.Events;
using Microsoft.Extensions.Logging;
using Muflone.Factories;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Transport.Azure.Consumers;
using Muflone.Transport.Azure.Models;

namespace BrewUpPubs.Modules.Pubs.Consumers.DomainEvents;

public sealed class BeerProductionStartedConsumer : DomainEventConsumerBase<BeerProductionStarted>
{
    protected override IEnumerable<IDomainEventHandlerAsync<BeerProductionStarted>> HandlersAsync { get; }

    public BeerProductionStartedConsumer(IDomainEventHandlerFactoryAsync domainEventHandlerFactoryAsync,
        AzureServiceBusConfiguration azureServiceBusConfiguration,
        ILoggerFactory loggerFactory, ISerializer? messageSerializer = null) : base(azureServiceBusConfiguration,
        loggerFactory, messageSerializer)
    {
        HandlersAsync = domainEventHandlerFactoryAsync.CreateDomainEventHandlersAsync<BeerProductionStarted>();
    }
}