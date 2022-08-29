using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace BrewUpPubs.Modules.Pubs.Abstracts;

public abstract class PubsDomainEventHandler<T> : IDomainEventHandlerAsync<T> where T : class, IDomainEvent
{
    protected ILogger Logger;

    protected PubsDomainEventHandler(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    public abstract Task HandleAsync(T @event, CancellationToken cancellationToken = new());

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~PubsDomainEventHandler()
    {
        Dispose(false);
    }
    #endregion
}