namespace eWatson.Abstractions.Events;

/// <summary>
/// Represents a domain event, capturing information about an occurrence
/// within the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }

    /// <summary>
    /// Gets the type name of the event, used for serialization and event identification.
    /// Default implementation returns the concrete type name.
    /// </summary>
    string EventType => GetType().Name;

    /// <summary>
    /// Gets the schema version of the event for handling event evolution.
    /// Defaults to version 1.
    /// </summary>
    int Version => 1;
}