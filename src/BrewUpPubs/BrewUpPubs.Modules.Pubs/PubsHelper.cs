using BrewUpPubs.Modules.Pubs.Abstracts;
using BrewUpPubs.Modules.Pubs.Concretes;
using BrewUpPubs.Modules.Pubs.EventsHandlers;
using BrewUpPubs.Modules.Pubs.Factories;
using BrewUpPubs.Modules.Pubs.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Muflone.Factories;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs;

public static class PubsHelper
{
    public static IServiceCollection AddPubsModule(this IServiceCollection services)
    {
        services.AddScoped<IBeerService, BeerService>();

        #region DomainEventHandler
        services.AddScoped<IDomainEventHandlerFactoryAsync, DomainEventHandlerFactoryAsync>();
        services.AddScoped<ICommandHandlerFactoryAsync, CommandHandlerFactoryAsync>();

        services
            .AddScoped<IDomainEventHandlerAsync<BeerProductionStarted>, BeerProductionStartedEventHandler>();

        services
            .AddScoped<IDomainEventHandlerAsync<BeerProductionCompleted>, BeerProductionCompletedEventHandler>();
        #endregion

        return services;
    }
}