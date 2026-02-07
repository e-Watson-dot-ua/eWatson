using System.Linq.Expressions;
using eWatson.Abstractions.Specifications.Pagination;

namespace eWatson.Abstractions.Specifications;

/// <summary>
/// Represents a specification that determines if an entity satisfies certain criteria.
/// Used to encapsulate business rules and query logic.
/// </summary>
/// <typeparam name="T">The type being evaluated.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the filter expression to apply to the query.
    /// Returns null if no filtering criteria should be applied.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the collection of navigation properties to eager load (Entity Framework).
    /// Used to optimize queries by reducing database round trips.
    /// </summary>
    IReadOnlyList<Include<T>> Includes { get; }

    /// <summary>
    /// Gets the collection of ordering expressions to apply to the query.
    /// Applied in the order they appear in the list.
    /// </summary>
    IReadOnlyList<OrderBy<T>> Orderings { get; }

    /// <summary>
    /// Gets the pagination parameters for limiting result sets.
    /// Returns null if pagination should not be applied.
    /// </summary>
    Paging? Paging { get; }

    /// <summary>
    /// Gets a value indicating whether the query should be executed without change tracking.
    /// Recommended for read-only queries to improve performance.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Gets a value indicating whether duplicate results should be filtered out.
    /// Applies DISTINCT clause to the query.
    /// </summary>
    bool Distinct { get; }
}
