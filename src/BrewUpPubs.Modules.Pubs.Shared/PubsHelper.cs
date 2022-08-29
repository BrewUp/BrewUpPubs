using BrewUpPubs.Shared.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace BrewUpPubs.Modules.Pubs.Shared;

public static class PubsHelper
{
    public static IServiceCollection AddPubs(this IServiceCollection services)
    {
        services.AddScoped<ValidationHandler>();
        //services.AddFluentValidation(options =>
        //    options.RegisterValidatorsFromAssemblyContaining<ProductionBeerValidator>());

        return services;
    }
}