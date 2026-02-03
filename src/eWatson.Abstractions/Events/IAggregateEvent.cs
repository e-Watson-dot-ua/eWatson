namespace eWatson.Abstractions.Events;

/// <summary>
/// Represents a domain event raised by an aggregate root.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IAggregateEvent<TId> : IDomainEvent
    where TId : notnull
{
    /// <summary>
    /// Gets the identifier of the aggregate that raised this event.
    /// </summary>
    TId AggregateId { get; }
}
