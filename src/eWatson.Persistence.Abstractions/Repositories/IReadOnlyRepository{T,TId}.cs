using eWatson.Abstractions.Entities;
using eWatson.Abstractions.Specifications;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Read-only repository interface for query operations.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IReadOnlyRepository<T, TId>
    where T : IAggregateRoot<TId>
    where TId : notnull, IEquatable<TId>
{
    /// <summary>
    /// Retrieves an aggregate by its unique identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The aggregate if found; otherwise, null.</returns>
    Task<T?> GetByIdAsync(TId id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all aggregates matching the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A collection of aggregates matching the specification.</returns>
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification, CancellationToken ct = default);

    /// <summary>
    /// Counts aggregates matching the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The count of aggregates matching the specification.</returns>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken ct = default);
}
