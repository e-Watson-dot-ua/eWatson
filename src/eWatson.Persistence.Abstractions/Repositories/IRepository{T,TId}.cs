using eWatson.Abstractions.Entities;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Full repository interface combining read and write operations.
/// Extends both <see cref="IReadRepository{T, TId}"/> and
/// <see cref="IWriteRepository{T}"/> for complete CRUD functionality.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
public interface IRepository<T, in TId> : IReadRepository<T, TId>, IWriteRepository<T>
    where T : IAggregateRoot<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// Checks if an aggregate with the specified identifier exists.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the aggregate exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
}
