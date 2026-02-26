namespace eWatson.Mediator.Abstractions;
/// <summary>
/// Controls how the mediator dispatches a notification to multiple handlers.
/// </summary>
public enum NotificationPublishStrategy
{
    /// <summary>
    /// Handlers are invoked one after another in registration order.
    /// If one handler throws, subsequent handlers are not invoked and the
    /// exception propagates to the caller.
    /// </summary>
    Sequential = 0,
    /// <summary>
    /// All handlers are invoked concurrently via <c>Task.WhenAll</c>.
    /// All exceptions are collected and re-thrown as an <see cref="System.AggregateException"/>.
    /// </summary>
    Parallel = 1,
    /// <summary>
    /// Handlers are invoked one after another in registration order.
    /// If a handler throws, the exception is recorded but the remaining
    /// handlers are still invoked. All collected exceptions are re-thrown
    /// as an <see cref="System.AggregateException"/> after all handlers complete.
    /// </summary>
    ContinueOnException = 2,
}
