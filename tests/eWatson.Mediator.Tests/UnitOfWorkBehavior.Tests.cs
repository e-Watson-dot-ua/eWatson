using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Persistence.Pipeline;
using eWatson.Mediator.Tests.Fakes;
using eWatson.Persistence.Abstractions.UnitOfWork;
using eWatson.Primitives.Results;

namespace eWatson.Mediator.Tests;

public sealed class UnitOfWorkBehaviorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task SuccessfulCommand_SavesAndCommits()
    {
        var behavior = new UnitOfWorkBehavior<FakeCommand, Result>(_unitOfWork);
        RequestContinuation<Result> next = _ => Task.FromResult(Result.Success());

        var result = await behavior.HandleAsync(new FakeCommand("test"), next);

        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FailedCommand_RollsBack()
    {
        var behavior = new UnitOfWorkBehavior<FakeCommand, Result>(_unitOfWork);
        RequestContinuation<Result> next = _ => Task.FromResult(Result.Failure("error"));

        var result = await behavior.HandleAsync(new FakeCommand("test"), next);

        result.IsFailure.Should().BeTrue();
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandlerThrows_RollsBackAndRethrows()
    {
        var behavior = new UnitOfWorkBehavior<FakeCommand, Result>(_unitOfWork);
        RequestContinuation<Result> next = _ => throw new InvalidOperationException("boom");

        var act = async () => await behavior.HandleAsync(new FakeCommand("test"), next);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NonResultResponse_PassesThrough_WithoutTransaction()
    {
        var behavior = new UnitOfWorkBehavior<FakeCommand, string>(_unitOfWork);
        RequestContinuation<string> next = _ => Task.FromResult("passthrough");

        var result = await behavior.HandleAsync(new FakeCommand("test"), next);

        result.Should().Be("passthrough");
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task QueryRequest_PassesThrough_WithoutTransaction()
    {
        var behavior = new UnitOfWorkBehavior<FakeQuery, Result<string>>(_unitOfWork);
        RequestContinuation<Result<string>> next = _ => Task.FromResult(Result.Success("data"));

        var result = await behavior.HandleAsync(new FakeQuery("term"), next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("data");
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValueCommand_SuccessResult_SavesAndCommits()
    {
        var behavior = new UnitOfWorkBehavior<FakeValueCommand, Result<int>>(_unitOfWork);
        RequestContinuation<Result<int>> next = _ => Task.FromResult(Result.Success(42));

        var result = await behavior.HandleAsync(new FakeValueCommand(21), next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValueCommand_FailureResult_RollsBack()
    {
        var behavior = new UnitOfWorkBehavior<FakeValueCommand, Result<int>>(_unitOfWork);
        RequestContinuation<Result<int>> next = _ => Task.FromResult(Result.Failure<int>("bad"));

        var result = await behavior.HandleAsync(new FakeValueCommand(21), next);

        result.IsFailure.Should().BeTrue();
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
