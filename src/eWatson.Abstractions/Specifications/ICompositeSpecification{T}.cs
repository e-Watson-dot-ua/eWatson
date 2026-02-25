namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Specification that can be combined with other specifications using logical operators.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
public interface ICompositeSpecification<T> : ISpecification<T>
{
    /// <summary>
    /// Combines this specification with another using AND logic.
    /// </summary>
    /// <param name="other">The specification to combine with.</param>
    /// <returns>A composite specification representing the AND operation.</returns>
    ICompositeSpecification<T> CombineWith(ISpecification<T> other);

    /// <summary>
    /// Combines this specification with another using OR logic.
    /// </summary>
    /// <param name="other">The specification to combine with.</param>
    /// <returns>A composite specification representing the OR operation.</returns>
    ICompositeSpecification<T> CombineWithOr(ISpecification<T> other);

    /// <summary>
    /// Negates this specification.
    /// </summary>
    /// <returns>A composite specification representing the NOT operation.</returns>
    ICompositeSpecification<T> Invert();
}
