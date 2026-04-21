using eWatson.Primitives.Results;
using FluentAssertions;

namespace eWatson.Primitives.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_IsSuccess()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Failure_HasErrorMessage()
    {
        var result = Result.Failure("something failed");

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("something failed");
    }

    [Fact]
    public void Failure_WithResultError_HasErrorDetails()
    {
        var error = ResultError.Validation("bad input", "Name");
        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.ErrorDetails.Should().NotBeNull();
        result.ErrorDetails!.Code.Should().Be("Validation.Error");
        result.Errors.Should().HaveCount(1);
    }

    [Fact]
    public void Failure_WithMultipleErrors_JoinsMessages()
    {
        var errors = new List<ResultError>
        {
            ResultError.Validation("error 1"),
            ResultError.Validation("error 2")
        };

        var result = Result.Failure(errors);

        result.ErrorMessage.Should().Contain("error 1");
        result.ErrorMessage.Should().Contain("error 2");
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void Success_WithValue_ReturnsValue()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_WithValue_ThrowsOnAccess()
    {
        var result = Result.Failure<int>("fail");

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ResultT_ImplicitConversion_FromValue()
    {
        Result<string> result = "hello";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ResultT_ImplicitConversion_FromError()
    {
        var error = ResultError.General("oops");
        Result<string> result = error;

        result.IsFailure.Should().BeTrue();
        result.ErrorDetails.Should().NotBeNull();
    }

    [Fact]
    public void Success_ErrorDetails_IsNull()
    {
        var result = Result.Success();

        result.ErrorDetails.Should().BeNull();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithNullErrors_Throws()
    {
        var act = () => Result.Failure((IReadOnlyList<ResultError>)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Failure_WithEmptyErrors_Throws()
    {
        var act = () => Result.Failure([]);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FailureT_WithNullErrors_Throws()
    {
        var act = () => Result.Failure<int>((IReadOnlyList<ResultError>)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FailureT_WithEmptyErrors_Throws()
    {
        var act = () => Result.Failure<int>([]);

        act.Should().Throw<ArgumentException>();
    }
}
