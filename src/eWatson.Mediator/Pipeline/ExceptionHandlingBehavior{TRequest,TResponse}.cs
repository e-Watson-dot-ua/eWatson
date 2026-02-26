using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Resources;
using eWatson.Results;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Outermost pipeline behavior that catches any unhandled exception thrown by inner
/// behaviors or the handler and converts it to a failure <see cref="Result"/>.
/// </summary>
/// <remarks>
/// This behavior only converts the exception when <typeparamref name="TResponse"/>
/// implements <see cref="IResult"/>. For non-Result responses the exception is re-thrown.
/// Register this as the <em>outermost</em> behavior so it guards the entire pipeline.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="ExceptionHandlingBehavior{TRequest,TResponse}"/>.
    /// </summary>
    public ExceptionHandlingBehavior() { }

    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        try
        {
            return await next(ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var requestName = typeof(TRequest).Name;

            // Only swallow to Result when the response IS a Result type.
            if (!typeof(IResult).IsAssignableFrom(typeof(TResponse)))
                throw;

            var error = ResultError.Internal(
                MediatorMessages.UnhandledException(requestName));

            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Failure(error);

            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result)
                    .GetMethod(nameof(Result.Failure), 1, [typeof(ResultError)])!
                    .MakeGenericMethod(valueType);

                return (TResponse)failureMethod.Invoke(null, [error])!;
            }

            throw;
        }
    }
}
