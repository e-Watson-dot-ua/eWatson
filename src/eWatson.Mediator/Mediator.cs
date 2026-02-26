using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Internal;
using eWatson.Mediator.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Mediator;

/// <summary>
/// Default implementation of <see cref="IMediator"/>.
/// Resolves handlers and pipeline behaviours from <see cref="IServiceProvider"/>
/// and composes them into an ordered delegate chain at dispatch time.
/// </summary>
/// <param name="serviceProvider">The DI service provider.</param>
/// <param name="options">Mediator configuration options.</param>
public sealed class Mediator(IServiceProvider serviceProvider, MediatorOptions options) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly MediatorOptions _options = options;

    /// <inheritdoc/>
    public Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var terminal = ResolveTerminal(request, requestType);
        var pipeline = BuildPipeline(request, requestType, terminal);

        return pipeline(ct);
    }

    /// <inheritdoc/>
    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken ct = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        switch (_options.PublishStrategy)
        {
            case NotificationPublishStrategy.Sequential:
                foreach (var handler in handlers)
                    await handler.HandleAsync(notification, ct).ConfigureAwait(false);
                break;

            case NotificationPublishStrategy.Parallel:
                var tasks = handlers
                    .Select(h => h.HandleAsync(notification, ct))
                    .ToList();
                await Task.WhenAll(tasks).ConfigureAwait(false);
                break;

            case NotificationPublishStrategy.ContinueOnException:
                var exceptions = new List<Exception>();
                foreach (var handler in handlers)
                {
                    try
                    {
                        await handler.HandleAsync(notification, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);
                break;

            default:
                throw new InvalidOperationException(
                    $"Notification publish strategy '{_options.PublishStrategy}' is not supported.");
        }
    }

    // -------------------------------------------------------------------------

    /// <summary>
    /// Resolves the registered handler for <paramref name="requestType"/> and wraps it in a
    /// type-erased delegate so it can be called without dynamic dispatch.
    /// </summary>
    private RequestContinuation<TResponse> ResolveTerminal<TResponse>(
        IRequest<TResponse> request,
        Type requestType)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handlerService = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                MediatorMessages.HandlerNotFound(requestType.Name));

        var wrapperType = typeof(HandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse));
        var wrapper = (IHandlerWrapper<TResponse>)Activator.CreateInstance(wrapperType, handlerService)!;

        return ct => wrapper.HandleAsync(request, ct);
    }

    /// <summary>
    /// Resolves all registered pipeline behaviours for <paramref name="requestType"/> and folds
    /// them around <paramref name="terminal"/>. The first registered behaviour becomes
    /// the outermost wrapper.
    /// </summary>
    private RequestContinuation<TResponse> BuildPipeline<TResponse>(
        IRequest<TResponse> request,
        Type requestType,
        RequestContinuation<TResponse> terminal)
    {
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = (IEnumerable<object>)(_serviceProvider.GetService(
            typeof(IEnumerable<>).MakeGenericType(behaviorType)) ?? Array.Empty<object>());

        var wrapperType = typeof(BehaviorWrapper<,>).MakeGenericType(requestType, typeof(TResponse));

        return behaviors
            .Reverse()
            .Aggregate(terminal, (next, behaviorService) =>
            {
                var wrapper = (IBehaviorWrapper<TResponse>)
                    Activator.CreateInstance(wrapperType, behaviorService)!;
                return ct => wrapper.HandleAsync(request, next, ct);
            });
    }
}
