using eWatson.Mediator.Abstractions;

namespace eWatson.Mediator.Internal;

/// <summary>
/// Type-erases the concrete request type so <see cref="Mediator"/> can invoke
/// a strongly-typed <see cref="IRequestHandler{TRequest,TResponse}"/> without dynamic dispatch.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the handler.</typeparam>
internal interface IHandlerWrapper<TResponse>
{
    Task<TResponse> HandleAsync(IRequest<TResponse> request, CancellationToken ct);
}

internal sealed class HandlerWrapper<TRequest, TResponse> : IHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _handler;

    public HandlerWrapper(IRequestHandler<TRequest, TResponse> handler) => _handler = handler;

    public Task<TResponse> HandleAsync(IRequest<TResponse> request, CancellationToken ct)
        => _handler.HandleAsync((TRequest)request, ct);
}

// -------------------------------------------------------------------------

/// <summary>
/// Type-erases the concrete request type so <see cref="Mediator"/> can invoke
/// a strongly-typed <see cref="IPipelineBehavior{TRequest,TResponse}"/> without dynamic dispatch.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the pipeline.</typeparam>
internal interface IBehaviorWrapper<TResponse>
{
    Task<TResponse> HandleAsync(
        IRequest<TResponse> request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct);
}

internal sealed class BehaviorWrapper<TRequest, TResponse> : IBehaviorWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IPipelineBehavior<TRequest, TResponse> _behavior;

    public BehaviorWrapper(IPipelineBehavior<TRequest, TResponse> behavior) => _behavior = behavior;

    public Task<TResponse> HandleAsync(
        IRequest<TResponse> request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct)
        => _behavior.HandleAsync((TRequest)request, continuation, ct);
}
