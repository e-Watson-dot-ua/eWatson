using eWatson.Abstractions.Entities;

namespace eWatson.Persistence.Abstractions.Repositories;

/// <summary>
/// Write repository interface for command operations on aggregate roots.
/// Provides add, update, and remove operations for CQRS command handling.
/// </summary>
/// <typeparam name="T">The aggregate root type.</typeparam>
public interface IWriteRepository<T> where T : IAggregateRoot
{
    /// <summary>
    /// Adds a new aggregate to the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(T aggregate, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing aggregate in the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to update.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(T aggregate, CancellationToken ct = default);

    /// <summary>
    /// Removes an aggregate from the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(T aggregate, CancellationToken ct = default);
}
