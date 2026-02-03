namespace eWatson.Abstractions.Entities;

/// <summary>
/// Marker interface for aggregate roots in the domain.
/// Aggregate roots are the only entities that can raise domain events
/// and serve as the consistency boundary for business invariants.
/// </summary>
public interface IAggregateRoot : IEntity, IHasDomainEvents
{
}
