using BrewUpPubs.ReadModel.Abstracts;
using BrewUpPubs.ReadModel.MongoDb.Repositories;
using BrewUpPubs.Shared.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace BrewUpPubs.ReadModel.MongoDb;

public static class MongoDbHelper
{
    public static IServiceCollection AddEventstoreMongoDb(this IServiceCollection services, MongoDbSettings mongoDbSettings)
    {
        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, MongoDbSettings mongoDbParameter)
    {
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoDbParameter.ConnectionString));
        services.AddScoped(provider =>
            provider.GetService<IMongoClient>()
                ?.GetDatabase(mongoDbParameter.DatabaseName)
                .WithWriteConcern(WriteConcern.W1));

        services.AddScoped<IFunctionalPersister, FunctionalPersister>();

        return services;
    }
}