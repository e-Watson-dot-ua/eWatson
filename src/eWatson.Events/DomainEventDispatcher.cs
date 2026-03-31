using eWatson.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eWatson.Events;

/// <summary>
/// Default implementation of <see cref="IDomainEventDispatcher"/> that resolves
/// <see cref="IDomainEventListener{TEvent}"/> instances from the service provider
/// and invokes them sequentially.
/// </summary>
internal sealed partial class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger = logger;

    /// <inheritdoc />
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var listenerType = typeof(IDomainEventListener<>).MakeGenericType(domainEvent.GetType());
        var listeners = _serviceProvider.GetServices(listenerType);

        var handleMethod = listenerType.GetMethod("HandleAsync")!;
        foreach (var listener in listeners)
        {
            LogDispatching(_logger, domainEvent.EventType, listener!.GetType().Name);
            var task = (Task)handleMethod.Invoke(listener, [domainEvent, ct])!;
            await task.ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, ct).ConfigureAwait(false);
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Dispatching {EventType} to {ListenerType}.")]
    private static partial void LogDispatching(ILogger logger, string eventType, string listenerType);
}
