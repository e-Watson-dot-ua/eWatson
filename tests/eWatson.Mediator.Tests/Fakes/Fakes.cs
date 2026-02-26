using eWatson.Mediator.Abstractions;
using eWatson.Results;
namespace eWatson.Mediator.Tests.Fakes;
// ---------------------------------------------------------------------------
// Fake command — void result
// ---------------------------------------------------------------------------
public sealed record FakeCommand(string Value) : ICommand;
public sealed class FakeCommandHandler : ICommandHandler<FakeCommand>
{
    public bool WasCalled { get; private set; }
    public Task<Result> HandleAsync(FakeCommand request, CancellationToken ct = default)
    {
        WasCalled = true;
        return Task.FromResult(Result.Success());
    }
}
// ---------------------------------------------------------------------------
// Fake command — valued result
// ---------------------------------------------------------------------------
public sealed record FakeValueCommand(int Input) : ICommand<int>;
public sealed class FakeValueCommandHandler : ICommandHandler<FakeValueCommand, int>
{
    public Task<Result<int>> HandleAsync(FakeValueCommand request, CancellationToken ct = default)
        => Task.FromResult(Result.Success(request.Input * 2));
}
// ---------------------------------------------------------------------------
// Fake query
// ---------------------------------------------------------------------------
public sealed record FakeQuery(string Term) : IQuery<string>;
public sealed class FakeQueryHandler : IQueryHandler<FakeQuery, string>
{
    public Task<Result<string>> HandleAsync(FakeQuery request, CancellationToken ct = default)
        => Task.FromResult(Result.Success($"result:{request.Term}"));
}
// ---------------------------------------------------------------------------
// Fake failing command
// ---------------------------------------------------------------------------
public sealed record FakeFailingCommand : ICommand;
public sealed class FakeFailingCommandHandler : ICommandHandler<FakeFailingCommand>
{
    public Task<Result> HandleAsync(FakeFailingCommand request, CancellationToken ct = default)
        => Task.FromResult(Result.Failure("forced failure"));
}
// ---------------------------------------------------------------------------
// Fake throwing command (for ExceptionHandlingBehavior tests)
// ---------------------------------------------------------------------------
public sealed record FakeThrowingCommand : ICommand;
public sealed class FakeThrowingCommandHandler : ICommandHandler<FakeThrowingCommand>
{
    public Task<Result> HandleAsync(FakeThrowingCommand request, CancellationToken ct = default)
        => throw new InvalidOperationException("boom");
}
// ---------------------------------------------------------------------------
// Fake notification
// ---------------------------------------------------------------------------
public sealed record FakeNotification(string Message) : INotification;
public sealed class FakeNotificationHandlerA : INotificationHandler<FakeNotification>
{
    public List<string> Received { get; } = [];
    public Task HandleAsync(FakeNotification notification, CancellationToken ct = default)
    {
        Received.Add(notification.Message + "_A");
        return Task.CompletedTask;
    }
}
public sealed class FakeNotificationHandlerB : INotificationHandler<FakeNotification>
{
    public List<string> Received { get; } = [];
    public Task HandleAsync(FakeNotification notification, CancellationToken ct = default)
    {
        Received.Add(notification.Message + "_B");
        return Task.CompletedTask;
    }
}
// ---------------------------------------------------------------------------
// Fake validator — rejects empty Value
// ---------------------------------------------------------------------------
public sealed class FakeCommandValidator : IRequestValidator<FakeCommand>
{
    public IReadOnlyList<eWatson.Results.ResultError> Validate(FakeCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
            return [ResultError.Validation("Value must not be empty.", "Value")];
        return [];
    }
}
