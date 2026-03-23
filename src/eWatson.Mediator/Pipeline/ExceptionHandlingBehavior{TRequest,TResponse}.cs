using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Internal;
using eWatson.Mediator.Resources;
using eWatson.Primitives.Results;
using Microsoft.Extensions.Logging;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Outermost pipeline behavior that catches any unhandled exception thrown by inner
/// behaviors or the handler and converts it to a failure <see cref="Result"/>.
/// </summary>
/// <remarks>
/// Only converts the exception when <typeparamref name="TResponse"/> implements
/// <see cref="IResult"/>. For non-Result responses the exception is re-thrown.
/// Register this as the <em>outermost</em> behavior so it guards the entire pipeline.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="logger">Logger for recording unhandled exceptions.</param>
public sealed partial class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
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

            LogUnhandledException(logger, typeof(TRequest).Name, ex);

            var error = ResultError.Internal(
                MediatorMessages.UnhandledException(typeof(TRequest).Name));

            return ResultHelper.CreateFailure<TResponse>(error);
        }
    }

    [LoggerMessage(Level = LogLevel.Error,
        Message = "Unhandled exception in handler for {RequestType}")]
    private static partial void LogUnhandledException(
        ILogger logger, string requestType, Exception ex);
}
