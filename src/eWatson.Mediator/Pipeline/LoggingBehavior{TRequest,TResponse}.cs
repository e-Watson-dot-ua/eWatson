using eWatson.Abstractions.Results;
using eWatson.Mediator.Abstractions;
using System.Diagnostics;

namespace eWatson.Mediator.Pipeline;

/// <summary>
/// Pipeline behavior that logs the request name, outcome, and elapsed time.
/// Logs at <see /> on success and <see /> on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Initializes a new instance of <see cref="LoggingBehavior{TRequest,TResponse}"/>.
    /// </summary>
    public LoggingBehavior() { }

    /// <inheritdoc/>
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var response = await next(ct).ConfigureAwait(false);
        sw.Stop();

        // Intentionally no logging to keep the package independent of logging providers.
        _ = response is IResult { IsFailure: true };
        _ = sw.ElapsedMilliseconds;

        return response;
    }
}
