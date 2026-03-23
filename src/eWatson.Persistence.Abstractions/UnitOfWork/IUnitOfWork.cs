namespace eWatson.Persistence.Abstractions.UnitOfWork;

/// <summary>
/// Represents a unit of work pattern for managing transactional boundaries.
/// </summary>
/// <remarks>
/// Intentionally does not extend <see cref="IDisposable"/> or <see cref="IAsyncDisposable"/>.
/// Lifetime management of the underlying context (e.g. <c>DbContext</c>) is delegated to the
/// DI container; callers should not dispose the unit of work directly.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of state entries written to the underlying data store.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
