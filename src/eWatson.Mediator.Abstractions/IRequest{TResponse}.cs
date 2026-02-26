namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Marker interface for a request that returns <typeparamref name="TResponse"/>
/// when dispatched through the mediator.
/// </summary>
/// <typeparam name="TResponse">The type of the response produced by this request.</typeparam>
public interface IRequest<out TResponse> { }
