namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Marker interface for a mediator notification.
/// Notifications are dispatched to zero or more handlers in a fan-out manner
/// via <see cref="IMediator.PublishAsync{TNotification}"/>.
/// </summary>
/// <remarks>
/// <para>
/// Notifications are distinct from domain events (<c>IDomainEvent</c>).
/// Domain events are raised inside aggregate roots and dispatched by infrastructure
/// <em>after</em> persistence. Notifications are application-layer concerns that
/// flow through the mediator at any point during request processing.
/// </para>
/// <para>
/// A common pattern is to bridge the two: publish a notification from inside
/// an <c>IDomainEventListener&lt;TEvent&gt;</c> implementation when cross-cutting
/// fan-out is required after a domain event is handled.
/// </para>
/// </remarks>
public interface INotification { }
