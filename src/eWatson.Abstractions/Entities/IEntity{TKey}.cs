namespace eWatson.Abstractions.Entities;

/// <summary>
/// Represents an entity with a strongly-typed unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
public interface IEntity<out TKey> : IEntity where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TKey Id { get; }
}