using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Extensions;
using eWatson.Mediator.Tests.Fakes;
using eWatson.Primitives.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eWatson.Mediator.Tests;

public sealed class PipelineBehaviorTests
{
    // -----------------------------------------------------------------------
    // ValidationBehavior
    // -----------------------------------------------------------------------
    [Fact]
    public async Task ValidationBehavior_ValidRequest_CallsHandler()
    {
        var services = new ServiceCollection();
        services.AddMediator(
            o => { o.EnableValidationBehavior = true; o.EnableLoggingBehavior = false; o.EnableExceptionHandlingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new FakeCommand("valid"));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_InvalidRequest_ShortCircuits_WithFailure()
    {
        var services = new ServiceCollection();
        services.AddMediator(
            o => { o.EnableValidationBehavior = true; o.EnableLoggingBehavior = false; o.EnableExceptionHandlingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new FakeCommand(""));

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Value must not be empty");
    }

    // -----------------------------------------------------------------------
    // ExceptionHandlingBehavior
    // -----------------------------------------------------------------------
    [Fact]
    public async Task ExceptionHandlingBehavior_HandlerThrows_Returns_FailureResult()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator(
            o => { o.EnableExceptionHandlingBehavior = true; o.EnableLoggingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new FakeThrowingCommand());

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ExceptionHandlingBehavior_OperationCanceled_IsNotSwallowed()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator(
            o => { o.EnableExceptionHandlingBehavior = true; o.EnableLoggingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);
        services.AddTransient<IRequestHandler<FakeCancelCommand, Result>, FakeCancelCommandHandler>();

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var act = async () => await mediator.SendAsync(new FakeCancelCommand());

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    // -----------------------------------------------------------------------
    // LoggingBehavior — smoke-test: ensure it does not change the result
    // -----------------------------------------------------------------------
    [Fact]
    public async Task LoggingBehavior_DoesNotAlterSuccessResult()
    {
        var services = new ServiceCollection();
        services.AddMediator(
            o => { o.EnableLoggingBehavior = true; o.EnableExceptionHandlingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new FakeQuery("smoke"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("result:smoke");
    }

    [Fact]
    public async Task LoggingBehavior_DoesNotAlterFailureResult()
    {
        var services = new ServiceCollection();
        services.AddMediator(
            o => { o.EnableLoggingBehavior = true; o.EnableExceptionHandlingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new FakeFailingCommand());

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("forced failure");
    }

    // -----------------------------------------------------------------------
    // Custom behaviour — pipeline ordering
    // -----------------------------------------------------------------------
    [Fact]
    public async Task CustomBehavior_ExecutesAroundHandler()
    {
        var log = new List<string>();

        var services = new ServiceCollection();
        services.AddMediator(
            o => { o.EnableLoggingBehavior = false; o.EnableExceptionHandlingBehavior = false; },
            typeof(PipelineBehaviorTests).Assembly);
        services.AddTransient<IPipelineBehavior<FakeCommand, Result>>(
            _ => new RecordingBehavior<FakeCommand, Result>(log, "custom"));

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        await mediator.SendAsync(new FakeCommand("order"));

        log.Should().Equal("custom:before", "custom:after");
    }
}

// ---------------------------------------------------------------------------
// Local fakes
// ---------------------------------------------------------------------------
file sealed record FakeCancelCommand : ICommand;
file sealed class FakeCancelCommandHandler : ICommandHandler<FakeCancelCommand>
{
    public Task<Result> HandleAsync(FakeCancelCommand request, CancellationToken ct = default)
        => throw new OperationCanceledException();
}
file sealed class RecordingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly List<string> _log;
    private readonly string _tag;
    public RecordingBehavior(List<string> log, string tag)
    {
        _log = log;
        _tag = tag;
    }
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestContinuation<TResponse> continuation,
        CancellationToken ct = default)
    {
        _log.Add($"{_tag}:before");
        var result = await continuation(ct);
        _log.Add($"{_tag}:after");
        return result;
    }
}
