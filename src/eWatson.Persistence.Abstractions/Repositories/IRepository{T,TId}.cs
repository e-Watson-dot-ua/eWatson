using eWatson.Abstractions.Entities;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Repository interface with strongly-typed identifier for aggregate roots.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IRepository<T, TId> : IRepository<T>
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
    /// Checks if an aggregate with the specified identifier exists.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the aggregate exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
}
