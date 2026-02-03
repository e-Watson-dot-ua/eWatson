using eWatson.Abstractions.Events;

namespace eWatson.Entities.Events;

/// <summary>
/// Abstract base record for domain events.
/// Domain events capture something that happened in the domain that is of interest to the business.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;

    // EventType and Version are provided by IDomainEvent default implementation
}
