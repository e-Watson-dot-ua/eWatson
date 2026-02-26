using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Internal;
using eWatson.Mediator.Resources;
using eWatson.Results;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Outermost pipeline behaviour that catches any unhandled exception thrown by inner
/// behaviours or the handler and converts it to a failure <see cref="Result"/>.
/// </summary>
/// <remarks>
/// Only converts the exception when <typeparamref name="TResponse"/> implements
/// <see cref="IResult"/>. For non-Result responses the exception is re-thrown.
/// Register this as the <em>outermost</em> behaviour so it guards the entire pipeline.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct = default)
    {
        try
        {
            return await continuation(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            if (!typeof(IResult).IsAssignableFrom(typeof(TResponse)))
                throw;

            var error = ResultError.Internal(
                MediatorMessages.UnhandledException(typeof(TRequest).Name));

            return ResultHelper.CreateFailure<TResponse>(error);
        }
    }
}
