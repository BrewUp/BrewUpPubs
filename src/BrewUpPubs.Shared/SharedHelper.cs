using BrewUpPubs.Shared.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUpPubs.Shared;

public static class SharedHelper
{
    public static IServiceCollection AddEventStore(this IServiceCollection services, EventStoreSettings eventStoreSettings)
    {

        return services;
    }
}