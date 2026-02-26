using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Resources;
using eWatson.Results;
using Microsoft.Extensions.DependencyInjection;
namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Pipeline behavior that runs all registered <see cref="IRequestValidator{TRequest}"/>
/// implementations and short-circuits the pipeline with a validation failure result
/// when any errors are found.
/// </summary>
/// <remarks>
/// Only short-circuits when <typeparamref name="TResponse"/> implements <see cref="IResult"/>.
/// For non-Result response types the validators are still run but failures throw
/// an <see cref="InvalidOperationException"/>.
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IRequestValidator<TRequest>> _validators;
    /// <summary>
    /// Initializes a new instance of <see cref="ValidationBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="serviceProvider">Used to resolve validators for the request type.</param>
    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _validators = serviceProvider.GetServices<IRequestValidator<TRequest>>();
    }
    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        var errors = _validators
            .SelectMany(v => v.Validate(request))
            .ToList();
        if (errors.Count == 0)
            return await next(ct).ConfigureAwait(false);
        var message = MediatorMessages.ValidationFailed(typeof(TRequest).Name, errors.Count);
        // Short-circuit: return a typed failure Result when the response is an IResult.
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(ResultError.Validation(message));
        // Attempt to construct Result<T> failure when TResponse is Result<T>.
        if (!typeof(TResponse).IsGenericType ||
            typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
            throw new InvalidOperationException(message);
        
        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(ResultError)])!
            .MakeGenericMethod(valueType);
        
        return (TResponse)failureMethod.Invoke(null, [ResultError.Validation(message)])!;
        // Fallback: non-Result response — throw so the exception behavior can catch it.
    }
}
