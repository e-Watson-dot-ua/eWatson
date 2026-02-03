namespace eWatson.Abstractions.Entities;

/// <summary>
/// Generic aggregate root with strongly-typed identifier.
/// Aggregate roots are the only entities that can raise domain events
/// and serve as the consistency boundary for business invariants.
/// </summary>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IAggregateRoot<TId> : IEntity<TId>, IAggregateRoot
    where TId : notnull, IEquatable<TId>
{
}
