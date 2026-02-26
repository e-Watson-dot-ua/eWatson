namespace eWatson.Mediator.Abstractions;
/// <summary>
/// Represents a pipeline behaviour (middleware) that wraps handler execution.
/// Register implementations in DI in the desired invocation order (outermost first).
/// </summary>
/// <typeparam name="TRequest">The request type flowing through the pipeline.</typeparam>
/// <typeparam name="TResponse">The response type produced by the pipeline.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Executes this behaviour and optionally calls the next step in the pipeline.
    /// </summary>
    /// <param name="request">The current request.</param>
    /// <param name="next">Delegate that invokes the next behaviour or the handler.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to the response.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default);
}
