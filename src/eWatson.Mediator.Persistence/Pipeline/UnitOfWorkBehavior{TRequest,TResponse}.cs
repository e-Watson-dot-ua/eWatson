using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using eWatson.Persistence.Abstractions.UnitOfWork;

namespace eWatson.Mediator.Persistence.Pipeline;

/// <summary>
/// Pipeline behaviour that wraps command execution in a database transaction.
/// </summary>
/// <remarks>
/// <para>
/// This behavior is only meaningful for commands (types implementing
/// <see cref="ICommand"/> or <see cref="ICommand{TValue}"/>), because queries
/// must not mutate state. Register it only for command pipelines, or rely on the fact
/// that it is a no-op when <typeparamref name="TResponse"/> is not an <see cref="IResult"/>.
/// </para>
/// <para>
/// Transaction lifecycle:
/// <list type="bullet">
///   <item>Begin transaction before calling the next pipeline step.</item>
///   <item>Commit transaction and save changes when the response is a successful <see cref="IResult"/>.</item>
///   <item>Roll back transaction when the response is a failed <see cref="IResult"/> or an exception escapes.</item>
/// </list>
/// </para>
/// <para>
/// Register this behavior <em>inside</em> the logging behaviour and <em>outside</em> the handler,
/// meaning it should be the last (innermost) behaviour registered:
/// <code>Exception → Logging → Validation → UnitOfWork → Handler</code>
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <remarks>
/// Initializes a new instance of <see cref="UnitOfWorkBehavior{TRequest,TResponse}"/>.
/// </remarks>
/// <param name="unitOfWork">The unit of work to manage the transaction boundary.</param>
public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct = default)
    {
        // Only manage a transaction when the response is a Result type.
        // Queries that return plain values pass through unchanged.
        if (!typeof(IResult).IsAssignableFrom(typeof(TResponse)))
            return await continuation(ct).ConfigureAwait(false);

        await _unitOfWork.BeginTransactionAsync(ct).ConfigureAwait(false);
        TResponse response;
        try
        {
            response = await continuation(ct).ConfigureAwait(false);
        }
        catch
        {
            await RollbackAsync(ct).ConfigureAwait(false);
            throw;
        }
        if (response is IResult { IsSuccess: true })
        {
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);
            await _unitOfWork.CommitTransactionAsync(ct).ConfigureAwait(false);
        }
        else
        {
            await RollbackAsync(ct).ConfigureAwait(false);
        }

        return response;
    }

    private async Task RollbackAsync(CancellationToken ct)
        => await _unitOfWork.RollbackTransactionAsync(ct).ConfigureAwait(false);
}
