namespace eWatson.Abstractions.Events;

/// <summary>
/// Handles a specific type of domain event.
/// </summary>
/// <remarks>
/// Implement this interface to react to a domain event raised within the domain.
/// Register implementations via your DI container.
/// </remarks>
/// <typeparam name="TEvent">The type of domain event to handle.</typeparam>
public interface IDomainEventListener<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
