namespace eWatson.Mediator.Abstractions;
/// <summary>
/// Handles a request of type <typeparamref name="TRequest"/> and produces
/// a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the specified request.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that resolves to the response.</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}
