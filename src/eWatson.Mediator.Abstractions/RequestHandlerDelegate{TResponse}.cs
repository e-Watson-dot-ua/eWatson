namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Represents the next step in the mediator pipeline — either the subsequent
/// <see cref="IPipelineBehavior{TRequest,TResponse}"/> or the final handler.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="ct">Cancellation token.</param>
/// <returns>A task that resolves to the response.</returns>
public delegate Task<TResponse> RequestContinuation<TResponse>(
    CancellationToken ct = default);
