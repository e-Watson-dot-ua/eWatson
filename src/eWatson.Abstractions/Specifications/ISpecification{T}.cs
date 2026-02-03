namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Represents a specification that determines if an entity satisfies certain criteria.
/// Used to encapsulate business rules and query logic.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Determines whether the specified entity satisfies the specification.
    /// </summary>
    /// <param name="entity">The entity to evaluate.</param>
    /// <returns>True if the entity satisfies the specification; otherwise, false.</returns>
    bool IsSatisfiedBy(T entity);
}
