using eWatson.Abstractions.Events;

namespace eWatson.Abstractions.Entities;

/// <summary>
/// Indicates that an entity has domain events.
/// Typically implemented by aggregate roots.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the domain events associated with the entity.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to the entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Removes a specific domain event from the entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Clears all domain events from the entity.
    /// Typically called by infrastructure after events are dispatched.
    /// </summary>
    void ClearDomainEvents();
}
