namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Handles a mediator notification of type <typeparamref name="TNotification"/>.
/// Multiple handlers for the same notification type are all invoked.
/// </summary>
/// <typeparam name="TNotification">The notification type to handle.</typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the specified notification.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TNotification notification, CancellationToken ct = default);
}
