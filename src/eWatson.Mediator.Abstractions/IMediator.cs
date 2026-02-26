namespace eWatson.Mediator.Abstractions;
/// <summary>
/// Dispatches requests to their registered handlers through the configured pipeline.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request through the pipeline to its single registered handler
    /// and returns the typed response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to the response.</returns>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken ct = default);
    /// <summary>
    /// Publishes a notification to all registered handlers in a fire-and-forget fan-out.
    /// Dispatch strategy (sequential, parallel, continue-on-exception) is controlled
    /// by <see cref="MediatorOptions"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing completion of all handlers.</returns>
    Task PublishAsync<TNotification>(TNotification notification,
        CancellationToken ct = default)
        where TNotification : INotification;
}
