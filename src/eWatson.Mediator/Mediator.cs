using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Resources;
using Microsoft.Extensions.DependencyInjection;
namespace eWatson.Mediator;
/// <summary>
/// Default implementation of <see cref="IMediator"/>.
/// Resolves handlers and pipeline behaviours from <see cref="IServiceProvider"/>
/// and composes them into an ordered delegate chain at dispatch time.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MediatorOptions _options;
    /// <summary>
    /// Initializes a new instance of <see cref="Mediator"/>.
    /// </summary>
    /// <param name="serviceProvider">The DI service provider.</param>
    /// <param name="options">Mediator configuration options.</param>
    public Mediator(IServiceProvider serviceProvider, MediatorOptions options)
    {
        _serviceProvider = serviceProvider;
        _options = options;
    }
    /// <inheritdoc/>
    public Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestType = request.GetType();
        // Resolve the concrete handler using the closed generic type.
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                MediatorMessages.HandlerNotFound(requestType.Name));
        // Build the terminal delegate that calls the actual handler.
        Task<TResponse> HandlerDelegate(CancellationToken innerCt) => (Task<TResponse>)((dynamic)handler).HandleAsync((dynamic)request, innerCt);
        // Resolve behaviours (registered as open-generic IEnumerable<IPipelineBehavior<,>>).
        var behaviourType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviours = (IEnumerable<object>)(_serviceProvider.GetService(
            typeof(IEnumerable<>).MakeGenericType(behaviourType)) ?? Array.Empty<object>());
        // Compose the pipeline: fold behaviours from innermost to outermost.
        // The last registered behaviour wraps the first — matching the order in which
        // behaviours are registered (outer first).
        var pipeline = behaviours
            .Reverse()
            .Aggregate(
                (RequestHandlerDelegate<TResponse>)HandlerDelegate,
                (next, behaviour) =>
                    innerCt => (Task<TResponse>)((dynamic)behaviour).HandleAsync(
                        (dynamic)request, next, innerCt));
        return pipeline(ct);
    }
    /// <inheritdoc/>
    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken ct = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        var handlers = _serviceProvider
            .GetServices<INotificationHandler<TNotification>>();
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
                throw new ArgumentOutOfRangeException(
                    nameof(_options.PublishStrategy),
                    _options.PublishStrategy,
                    null);
        }
    }
}
