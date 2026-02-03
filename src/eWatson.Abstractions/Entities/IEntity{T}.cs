namespace eWatson.Abstractions.Entities;

/// <summary>
/// Represents an entity with a strongly-typed unique identifier.
/// </summary>
/// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
public interface IEntity<TKey> : IEntity where TKey : notnull, IEquatable<TKey>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TKey Id { get; }
}