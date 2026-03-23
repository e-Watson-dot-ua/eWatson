using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Internal;
using eWatson.Primitives.Results;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Pipeline behavior that runs all registered <see cref="IRequestValidator{TRequest}"/>
/// implementations and short-circuits the pipeline with a validation failure result
/// when any errors are found.
/// </summary>
/// <remarks>
/// <para>
/// Only short-circuits when <typeparamref name="TResponse"/> is <see cref="Result"/> or
/// <see cref="Result{T}"/>. For other response types <see cref="ResultHelper.CreateFailure{TResponse}(ResultError)"/>
/// will throw an <see cref="InvalidOperationException"/>.
/// </para>
/// <para>
/// All individual validation errors are preserved in the result's
/// <see cref="Result.Errors"/> collection.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="serviceProvider">Used to resolve validators for the request type.</param>
public sealed class ValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IRequestValidator<TRequest>> _validators
        = serviceProvider.GetServices<IRequestValidator<TRequest>>();

    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct = default)
    {
        var errors = _validators
            .SelectMany(v => v.Validate(request))
            .ToList();

        if (errors.Count == 0)
            return await continuation(ct).ConfigureAwait(false);

        return ResultHelper.CreateFailure<TResponse>(errors);
    }
}
