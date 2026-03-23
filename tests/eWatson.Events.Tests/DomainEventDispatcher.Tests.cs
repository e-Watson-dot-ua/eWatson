using eWatson.Abstractions.Events;
using eWatson.Entities.Events;
using eWatson.Events.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Events.Tests;

public sealed class DomainEventDispatcherTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------
    private static IDomainEventDispatcher BuildDispatcher(
        Action<IServiceCollection>? extra = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDomainEventDispatching(typeof(DomainEventDispatcherTests).Assembly);
        extra?.Invoke(services);
        return services.BuildServiceProvider().GetRequiredService<IDomainEventDispatcher>();
    }

    // -----------------------------------------------------------------------
    // Single event dispatch
    // -----------------------------------------------------------------------
    [Fact]
    public async Task DispatchAsync_SingleEvent_InvokesMatchingListener()
    {
        var dispatcher = BuildDispatcher();

        await dispatcher.DispatchAsync(new OrderCreatedEvent("ORD-1"));

        RecordingOrderCreatedListener.Received.Should().ContainSingle()
            .Which.OrderNumber.Should().Be("ORD-1");
    }

    [Fact]
    public async Task DispatchAsync_SingleEvent_DoesNotInvokeUnrelatedListener()
    {
        var dispatcher = BuildDispatcher();

        await dispatcher.DispatchAsync(new OrderCreatedEvent("ORD-2"));

        RecordingOrderCancelledListener.Received.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // Multiple events dispatch
    // -----------------------------------------------------------------------
    [Fact]
    public async Task DispatchAsync_MultipleEvents_InvokesCorrectListeners()
    {
        var dispatcher = BuildDispatcher();

        var events = new IDomainEvent[]
        {
            new OrderCreatedEvent("ORD-3"),
            new OrderCancelledEvent("ORD-4")
        };

        await dispatcher.DispatchAsync(events);

        RecordingOrderCreatedListener.Received.Should().ContainSingle()
            .Which.OrderNumber.Should().Be("ORD-3");
        RecordingOrderCancelledListener.Received.Should().ContainSingle()
            .Which.OrderNumber.Should().Be("ORD-4");
    }

    // -----------------------------------------------------------------------
    // Multiple listeners for same event
    // -----------------------------------------------------------------------
    [Fact]
    public async Task DispatchAsync_MultipleListeners_AllInvoked()
    {
        var dispatcher = BuildDispatcher();

        await dispatcher.DispatchAsync(new OrderCreatedEvent("ORD-5"));

        RecordingOrderCreatedListener.Received.Should().Contain(e => e.OrderNumber == "ORD-5");
        SecondOrderCreatedListener.Received.Should().Contain(e => e.OrderNumber == "ORD-5");
    }

    // -----------------------------------------------------------------------
    // No listeners — does not throw
    // -----------------------------------------------------------------------
    [Fact]
    public async Task DispatchAsync_NoListeners_DoesNotThrow()
    {
        // Build without assembly scanning so no listeners are registered.
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDomainEventDispatching();
        var dispatcher = services.BuildServiceProvider()
            .GetRequiredService<IDomainEventDispatcher>();

        var act = async () => await dispatcher.DispatchAsync(new OrderCreatedEvent("ORD-6"));

        await act.Should().NotThrowAsync();
    }

    // -----------------------------------------------------------------------
    // Null guards
    // -----------------------------------------------------------------------
    [Fact]
    public async Task DispatchAsync_NullEvent_Throws_ArgumentNullException()
    {
        var dispatcher = BuildDispatcher();
        var act = async () => await dispatcher.DispatchAsync((IDomainEvent)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DispatchAsync_NullEvents_Throws_ArgumentNullException()
    {
        var dispatcher = BuildDispatcher();
        var act = async () => await dispatcher.DispatchAsync((IEnumerable<IDomainEvent>)null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    // -----------------------------------------------------------------------
    // DI registration — AddDomainEventDispatching scans assemblies
    // -----------------------------------------------------------------------
    [Fact]
    public void AddDomainEventDispatching_RegistersDispatcherAsScoped()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDomainEventDispatching();

        var descriptor = services.FirstOrDefault(
            d => d.ServiceType == typeof(IDomainEventDispatcher));

        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddDomainEventDispatching_ScansListenersFromAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDomainEventDispatching(typeof(DomainEventDispatcherTests).Assembly);

        var listeners = services.Where(
            d => d.ServiceType.IsGenericType
              && d.ServiceType.GetGenericTypeDefinition() == typeof(IDomainEventListener<>));

        listeners.Should().NotBeEmpty();
    }

    // -----------------------------------------------------------------------
    // Setup / teardown — clear static state between tests
    // -----------------------------------------------------------------------
    public DomainEventDispatcherTests()
    {
        RecordingOrderCreatedListener.Received.Clear();
        RecordingOrderCancelledListener.Received.Clear();
        SecondOrderCreatedListener.Received.Clear();
    }
}

// ---------------------------------------------------------------------------
// Test domain events
// ---------------------------------------------------------------------------
public sealed record OrderCreatedEvent(string OrderNumber) : DomainEvent;
public sealed record OrderCancelledEvent(string OrderNumber) : DomainEvent;

// ---------------------------------------------------------------------------
// Test listeners
// ---------------------------------------------------------------------------
public sealed class RecordingOrderCreatedListener : IDomainEventListener<OrderCreatedEvent>
{
    public static List<OrderCreatedEvent> Received { get; } = [];

    public Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken ct = default)
    {
        Received.Add(domainEvent);
        return Task.CompletedTask;
    }
}

public sealed class SecondOrderCreatedListener : IDomainEventListener<OrderCreatedEvent>
{
    public static List<OrderCreatedEvent> Received { get; } = [];

    public Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken ct = default)
    {
        Received.Add(domainEvent);
        return Task.CompletedTask;
    }
}

public sealed class RecordingOrderCancelledListener : IDomainEventListener<OrderCancelledEvent>
{
    public static List<OrderCancelledEvent> Received { get; } = [];

    public Task HandleAsync(OrderCancelledEvent domainEvent, CancellationToken ct = default)
    {
        Received.Add(domainEvent);
        return Task.CompletedTask;
    }
}
