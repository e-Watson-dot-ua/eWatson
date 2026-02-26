using eWatson.Abstractions.Entities;

namespace eWatson.Entities;

/// <summary>
/// Abstract base class for entities with a strongly-typed identifier.
/// Provides identity-based equality semantics.
/// </summary>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
public abstract class Entity<TId> : IEntity<TId>
    where TId : IEquatable<TId>
{
    protected Entity() { }
    protected Entity(TId id) => Id = id;

    /// <summary>
    /// Gets or initializes the unique identifier of the entity.
    /// </summary>
    public TId Id { get; protected init; } = default!;

    /// <summary>
    /// Determines whether two entities are equal based on their identifiers.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns>True if the specified object is equal to the current entity; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient() || other.IsTransient())
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns the hash code for this entity based on its identifier.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified entities are equal.
    /// </summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether the specified entities are not equal.
    /// </summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Checks if the entity is transient (not yet persisted).
    /// An entity is transient if its ID equals the default value.
    /// </summary>
    /// <returns>True if the entity is transient; otherwise, false.</returns>
    private bool IsTransient()
    {
        return Id.Equals(default);
    }
}
