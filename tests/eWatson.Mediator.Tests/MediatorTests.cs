using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Extensions;
using eWatson.Mediator.Tests.Fakes;
using eWatson.Results;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Mediator.Tests;

public sealed class MediatorTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------
    private static IMediator BuildMediator(
        Action<IServiceCollection>? extra = null,
        Action<MediatorOptions>? configure = null)
    {
        var services = new ServiceCollection();

        services.AddEWatsonMediator(
            configure,
            typeof(MediatorTests).Assembly);

        extra?.Invoke(services);

        return services.BuildServiceProvider().GetRequiredService<IMediator>();
    }
    // -----------------------------------------------------------------------
    // Send — ICommand (void Result)
    // -----------------------------------------------------------------------
    [Fact]
    public async Task SendAsync_VoidCommand_Returns_SuccessResult()
    {
        var mediator = BuildMediator();
        var result = await mediator.SendAsync(new FakeCommand("hello"));
        result.IsSuccess.Should().BeTrue();
    }
    [Fact]
    public async Task SendAsync_VoidCommand_Invokes_Handler()
    {
        var handler = new FakeCommandHandler();
        var services = new ServiceCollection();
        services.AddSingleton<MediatorOptions>(new MediatorOptions
        {
            EnableExceptionHandlingBehavior = false,
            EnableLoggingBehavior = false
        });
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        services.AddTransient<IRequestHandler<FakeCommand, Result>>(_ => handler);
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        await mediator.SendAsync(new FakeCommand("hello"));

        handler.WasCalled.Should().BeTrue();
    }
    // -----------------------------------------------------------------------
    // Send — ICommand<TValue> (Result<T>)
    // -----------------------------------------------------------------------
    [Fact]
    public async Task SendAsync_ValueCommand_Returns_CorrectValue()
    {
        var mediator = BuildMediator();
        var result = await mediator.SendAsync(new FakeValueCommand(21));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }
    // -----------------------------------------------------------------------
    // Send — IQuery<TResponse>
    // -----------------------------------------------------------------------
    [Fact]
    public async Task SendAsync_Query_Returns_CorrectValue()
    {
        var mediator = BuildMediator();
        var result = await mediator.SendAsync(new FakeQuery("test"));
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("result:test");
    }
    // -----------------------------------------------------------------------
    // Send — no handler registered
    // -----------------------------------------------------------------------
    [Fact]
    public async Task SendAsync_NoHandler_Throws_InvalidOperationException()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new MediatorOptions
        {
            EnableExceptionHandlingBehavior = false,
            EnableLoggingBehavior = false
        });
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var act = async () => await mediator.SendAsync(new FakeCommand("x"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*FakeCommand*");
    }
    // -----------------------------------------------------------------------
    // Publish — sequential (default)
    // -----------------------------------------------------------------------
    [Fact]
    public async Task PublishAsync_Sequential_InvokesAllHandlers()
    {
        var handlerA = new FakeNotificationHandlerA();
        var handlerB = new FakeNotificationHandlerB();

        var services = new ServiceCollection();
        services.AddSingleton(new MediatorOptions());
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        services.AddTransient<INotificationHandler<FakeNotification>>(_ => handlerA);
        services.AddTransient<INotificationHandler<FakeNotification>>(_ => handlerB);
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        await mediator.PublishAsync(new FakeNotification("ping"));

        handlerA.Received.Should().ContainSingle("ping_A");
        handlerB.Received.Should().ContainSingle("ping_B");
    }
    // -----------------------------------------------------------------------
    // Publish — parallel
    // -----------------------------------------------------------------------
    [Fact]
    public async Task PublishAsync_Parallel_InvokesAllHandlers()
    {
        var handlerA = new FakeNotificationHandlerA();
        var handlerB = new FakeNotificationHandlerB();

        var services = new ServiceCollection();
        services.AddSingleton(new MediatorOptions
        {
            PublishStrategy = NotificationPublishStrategy.Parallel
        });
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        services.AddTransient<INotificationHandler<FakeNotification>>(_ => handlerA);
        services.AddTransient<INotificationHandler<FakeNotification>>(_ => handlerB);
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        await mediator.PublishAsync(new FakeNotification("ping"));

        handlerA.Received.Should().ContainSingle("ping_A");
        handlerB.Received.Should().ContainSingle("ping_B");
    }
    // -----------------------------------------------------------------------
    // Publish — ContinueOnException collects all exceptions
    // -----------------------------------------------------------------------
    [Fact]
    public async Task PublishAsync_ContinueOnException_CollectsAllExceptions()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new MediatorOptions
        {
            PublishStrategy = NotificationPublishStrategy.ContinueOnException
        });
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        services.AddTransient<INotificationHandler<FakeNotification>>(
            _ => new ThrowingNotificationHandler("first"));
        services.AddTransient<INotificationHandler<FakeNotification>>(
            _ => new ThrowingNotificationHandler("second"));
        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var act = async () => await mediator.PublishAsync(new FakeNotification("x"));

        var ex = (await act.Should().ThrowAsync<AggregateException>()).Which;
        ex.InnerExceptions.Should().HaveCount(2);
    }
    // -----------------------------------------------------------------------
    // Null guard
    // -----------------------------------------------------------------------
    [Fact]
    public async Task SendAsync_NullRequest_Throws_ArgumentNullException()
    {
        var mediator = BuildMediator();
        var act = async () => await mediator.SendAsync<Result>(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
// ---------------------------------------------------------------------------
// Local helper — notification handler that always throws
// ---------------------------------------------------------------------------
file sealed class ThrowingNotificationHandler : INotificationHandler<FakeNotification>
{
    private readonly string _name;
    public ThrowingNotificationHandler(string name) => _name = name;
    public Task HandleAsync(FakeNotification notification, CancellationToken ct = default)
        => throw new InvalidOperationException(_name);
}
