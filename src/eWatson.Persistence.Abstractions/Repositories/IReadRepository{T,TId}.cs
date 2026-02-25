using eWatson.Abstractions.Entities;
using eWatson.Abstractions.Specifications;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Read repository interface for query operations on aggregate roots.
/// Provides read-only access for CQRS query handling.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IReadRepository<T, in TId>
    where T : IAggregateRoot<TId>
    where TId : IEquatable<TId>
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
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Counts aggregates matching the specification.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The count of aggregates matching the specification.</returns>
    Task<int> CountAsync(ISpecification<T> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Determines whether any aggregate matches the specification.
    /// More efficient than <see cref="CountAsync"/> when only existence is needed.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if at least one aggregate matches; otherwise, <c>false</c>.</returns>
    Task<bool> AnyAsync(ISpecification<T> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves the first aggregate matching the specification, or <c>null</c>
    /// if no match is found.
    /// </summary>
    /// <param name="specification">The specification to match.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The first matching aggregate, or <c>null</c>.</returns>
    Task<T?> FindFirstAsync(ISpecification<T> specification,
        CancellationToken ct = default);
}
