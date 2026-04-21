using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Extensions;
using eWatson.Mediator.Persistence.Extensions;
using eWatson.Persistence.Abstractions.UnitOfWork;
using eWatson.Primitives.Results;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Mediator.Tests;

public sealed class MediatorPipelineIntegrationTests
{
    [Fact]
    public async Task InvalidCommand_ReturnsStructuredValidationErrors_AndSkipsUnitOfWork()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator(
            o =>
            {
                o.EnableValidationBehavior = true;
                o.EnableLoggingBehavior = false;
                o.EnableExceptionHandlingBehavior = true;
            },
            typeof(MediatorPipelineIntegrationTests).Assembly);
        services.AddMediatorUnitOfWork();
        services.AddSingleton(unitOfWork);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new PipelineValueCommand(string.Empty));

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(2);
        result.Errors.Select(e => e.Code).Should().Contain(["Validation.Error", "Conflict.Error"]);
        await unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ValidCommand_CommitsUnitOfWork_AndReturnsValue()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator(
            o =>
            {
                o.EnableValidationBehavior = true;
                o.EnableLoggingBehavior = false;
                o.EnableExceptionHandlingBehavior = true;
            },
            typeof(MediatorPipelineIntegrationTests).Assembly);
        services.AddMediatorUnitOfWork();
        services.AddSingleton(unitOfWork);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new PipelineValueCommand("alpha"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(5);
        await unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandlerException_RollsBackUnitOfWork_AndReturnsFailureResult()
    {
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediator(
            o =>
            {
                o.EnableValidationBehavior = false;
                o.EnableLoggingBehavior = false;
                o.EnableExceptionHandlingBehavior = true;
            },
            typeof(MediatorPipelineIntegrationTests).Assembly);
        services.AddMediatorUnitOfWork();
        services.AddSingleton(unitOfWork);

        var mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new ThrowingPipelineCommand("boom"));

        result.IsFailure.Should().BeTrue();
        result.ErrorDetails.Should().NotBeNull();
        result.ErrorDetails!.Code.Should().Be("Internal.Error");
        await unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }
}

file sealed record PipelineValueCommand(string Value) : ICommand<int>;

file sealed class PipelineValueCommandHandler : ICommandHandler<PipelineValueCommand, int>
{
    public Task<Result<int>> HandleAsync(PipelineValueCommand request, CancellationToken ct = default)
        => Task.FromResult(Result.Success(request.Value.Length));
}

file sealed class PipelineValueCommandValidator : IRequestValidator<PipelineValueCommand>
{
    public IReadOnlyList<ResultError> Validate(PipelineValueCommand request)
    {
        var errors = new List<ResultError>();

        if (string.IsNullOrWhiteSpace(request.Value))
        {
            errors.Add(ResultError.Validation("Value must not be empty.", "Value"));
            errors.Add(ResultError.Conflict("Value must be unique."));
        }

        return errors;
    }
}

file sealed record ThrowingPipelineCommand(string Value) : ICommand<int>;

file sealed class ThrowingPipelineCommandHandler : ICommandHandler<ThrowingPipelineCommand, int>
{
    public Task<Result<int>> HandleAsync(ThrowingPipelineCommand request, CancellationToken ct = default)
        => throw new InvalidOperationException(request.Value);
}
