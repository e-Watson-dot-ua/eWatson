namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Configuration options for the mediator, applied during DI registration
/// via <c>AddMediator</c>.
/// </summary>
public sealed class MediatorOptions
{
    /// <summary>
    /// Gets or sets the strategy used when publishing a notification to multiple handlers.
    /// Defaults to <see cref="NotificationPublishStrategy.Sequential"/>.
    /// </summary>
    public NotificationPublishStrategy PublishStrategy { get; set; } =
        NotificationPublishStrategy.Sequential;

    /// <summary>
    /// Gets or sets a value indicating whether the built-in exception-handling behaviour
    /// is registered. When <see langword="true"/>, unhandled exceptions from handlers
    /// are caught and converted to a failure <c>Result</c>.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool EnableExceptionHandlingBehavior { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the built-in logging behaviour is registered.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool EnableLoggingBehavior { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the built-in validation behaviour is registered.
    /// When <see langword="true"/>, all <c>IRequestValidator&lt;TRequest&gt;</c>
    /// implementations registered in DI are executed before the handler.
    /// Defaults to <see langword="false"/>.
    /// </summary>
    public bool EnableValidationBehavior { get; set; }
}
