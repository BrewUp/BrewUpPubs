using BrewUpPubs.ReadModel.Abstracts;
using Microsoft.Extensions.Logging;

namespace BrewUpPubs.Modules.Pubs.Abstracts;

public abstract class PubsBaseService
{
    protected readonly IFunctionalPersister FunctionalPersister;
    protected readonly ILogger Logger;

    protected PubsBaseService(IFunctionalPersister functionalPersister,
        ILoggerFactory loggerFactory)
    {
        FunctionalPersister = functionalPersister;
        Logger = loggerFactory.CreateLogger(GetType());
    }
}