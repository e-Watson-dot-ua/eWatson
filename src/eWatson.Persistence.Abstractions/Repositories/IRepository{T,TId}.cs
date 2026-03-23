using eWatson.Abstractions.Entities;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Full repository interface combining read and write operations.
/// Extends both <see cref="IReadRepository{T, TId}"/> and
/// <see cref="IWriteRepository{T}"/> for complete CRUD functionality.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
/// <typeparam name="TId">The type of the aggregate identifier.</typeparam>
/// <remarks>
/// <para>
/// <typeparamref name="T"/> is constrained to <see cref="IAggregateRoot{TId}"/> by design.
/// Only aggregate roots own their transactional boundary and are valid repository targets in DDD.
/// Entities that are not aggregate roots should be accessed through the repository of
/// their owning aggregate root rather than through their own repository.
/// </para>
/// <para>
/// If you need to promote an <c>Entity&lt;TId&gt;</c> to use this interface, inherit from
/// <c>AggregateRoot&lt;TId&gt;</c> instead — this is an intentional DDD modelling decision,
/// not a workaround.
/// </para>
/// </remarks>
public interface IRepository<T, in TId> : IReadRepository<T, TId>, IWriteRepository<T>
    where T : IAggregateRoot<TId> where TId : IEquatable<TId>
{
    /// <summary>
    /// Checks if an aggregate with the specified identifier exists.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if the aggregate exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken ct = default);
}
