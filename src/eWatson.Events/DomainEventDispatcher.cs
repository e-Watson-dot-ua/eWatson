using eWatson.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

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
    private delegate Task DispatchDelegate(
        IServiceProvider serviceProvider,
        IDomainEvent domainEvent,
        ILogger logger,
        CancellationToken ct);

    private static readonly ConcurrentDictionary<Type, DispatchDelegate> Dispatchers = [];
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger = logger;

    /// <inheritdoc />
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var dispatcher = Dispatchers.GetOrAdd(domainEvent.GetType(), CreateDispatcher);
        await dispatcher(_serviceProvider, domainEvent, _logger, ct).ConfigureAwait(false);
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

    private static DispatchDelegate CreateDispatcher(Type eventType)
    {
        var method = typeof(DomainEventDispatcher)
            .GetMethod(nameof(DispatchTypedAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .MakeGenericMethod(eventType);

        return method.CreateDelegate<DispatchDelegate>();
    }

    private static async Task DispatchTypedAsync<TEvent>(
        IServiceProvider serviceProvider,
        IDomainEvent domainEvent,
        ILogger logger,
        CancellationToken ct)
        where TEvent : IDomainEvent
    {
        var typedEvent = (TEvent)domainEvent;
        var listeners = serviceProvider.GetServices<IDomainEventListener<TEvent>>();

        foreach (var listener in listeners)
        {
            LogDispatching(logger, typedEvent.EventType, listener.GetType().Name);
            await listener.HandleAsync(typedEvent, ct).ConfigureAwait(false);
        }
    }
}
