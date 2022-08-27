using BrewUpPubs.Modules.Pubs;
using BrewUpPubs.Modules.Pubs.Consumers.DomainEvents;
using BrewUpPubs.Modules.Pubs.Shared.Events;
using BrewUpPubs.ReadModel.MongoDb;
using BrewUpPubs.Shared;
using BrewUpPubs.Shared.Configuration;
using Muflone.Factories;
using Muflone.Transport.Azure;
using Muflone.Transport.Azure.Abstracts;
using Muflone.Transport.Azure.Models;

namespace BrewUpPubs.Modules;

public class InfrastructureModule : IModule
{
    public bool IsEnabled => true;
    public int Order => 98;

    public IServiceCollection RegisterModule(WebApplicationBuilder builder)
    {
        builder.Services.AddPubsModule();

        builder.Services.AddEventStore(builder.Configuration.GetSection("BrewUp:EventStoreSettings").Get<EventStoreSettings>());

        var serviceProvider = builder.Services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        var domainEventHandlerFactoryAsync = serviceProvider.GetService<IDomainEventHandlerFactoryAsync>();

        var clientId = builder.Configuration["BrewUp:ClientId"];
        var serviceBusConnectionString = builder.Configuration["BrewUp:ServiceBusSettings:ConnectionString"];
        var consumers = new List<IConsumer>
        {
            new BeerProductionStartedConsumer(domainEventHandlerFactoryAsync!, new AzureServiceBusConfiguration(serviceBusConnectionString, nameof(BeerProductionStarted), clientId), loggerFactory!),
            new BeerProductionCompletedConsumer(domainEventHandlerFactoryAsync!, new AzureServiceBusConfiguration(serviceBusConnectionString, nameof(BeerProductionCompleted), clientId), loggerFactory!)
        };
        builder.Services.AddMufloneTransportAzure(
            new AzureServiceBusConfiguration(builder.Configuration["BrewUp:ServiceBusSettings:ConnectionString"], "",
                clientId), consumers);
        
        var mongoDbSettings = new MongoDbSettings();
        builder.Configuration.GetSection("BrewUp:MongoDbSettings").Bind(mongoDbSettings);
        builder.Services.AddEventstoreMongoDb(mongoDbSettings);

        return builder.Services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints;
}