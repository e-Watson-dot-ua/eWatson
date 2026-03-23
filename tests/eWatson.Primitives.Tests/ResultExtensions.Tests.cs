using eWatson.Primitives.Maybes;
using eWatson.Primitives.Results;
using FluentAssertions;

namespace eWatson.Primitives.Tests;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void Match_Success_CallsOnSuccess()
    {
        var result = Result.Success(10);

        var output = result.Match(
            v => v * 2,
            _ => -1);

        output.Should().Be(20);
    }

    [Fact]
    public void Match_Failure_CallsOnFailure()
    {
        var result = Result.Failure<int>("error");

        var output = result.Match(
            v => v * 2,
            err => -1);

        output.Should().Be(-1);
    }

    [Fact]
    public void Map_Success_TransformsValue()
    {
        var result = Result.Success(5);

        var mapped = result.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be("5");
    }

    [Fact]
    public void Map_Failure_PropagatesError()
    {
        var result = Result.Failure<int>("fail");

        var mapped = result.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.IsFailure.Should().BeTrue();
        mapped.ErrorMessage.Should().Be("fail");
    }

    [Fact]
    public void Bind_Success_ChainsResults()
    {
        var result = Result.Success(10);

        var bound = result.Bind(v => Result.Success(v + 5));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be(15);
    }

    [Fact]
    public void Bind_Failure_ShortCircuits()
    {
        var result = Result.Failure<int>("fail");

        var bound = result.Bind(v => Result.Success(v + 5));

        bound.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Tap_Success_ExecutesAction()
    {
        var tapped = false;
        var result = Result.Success(1);

        var returned = result.Tap(_ => tapped = true);

        tapped.Should().BeTrue();
        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void TapError_Failure_ExecutesAction()
    {
        string? errorMsg = null;
        var result = Result.Failure<int>("bad");

        result.TapError(e => errorMsg = e);

        errorMsg.Should().Be("bad");
    }

    [Fact]
    public void Ensure_PassingPredicate_ReturnsSameResult()
    {
        var result = Result.Success(10);

        var ensured = result.Ensure(v => v > 0, "must be positive");

        ensured.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Ensure_FailingPredicate_ReturnsFailure()
    {
        var result = Result.Success(-1);

        var ensured = result.Ensure(v => v > 0, "must be positive");

        ensured.IsFailure.Should().BeTrue();
        ensured.ErrorMessage.Should().Be("must be positive");
    }

    [Fact]
    public void ToMaybe_Success_ReturnsSome()
    {
        var result = Result.Success("hello");

        var maybe = result.ToMaybe();

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be("hello");
    }

    [Fact]
    public void ToMaybe_Failure_ReturnsNone()
    {
        var result = Result.Failure<string>("fail");

        var maybe = result.ToMaybe();

        maybe.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Combine_AllSuccess_ReturnsAllValues()
    {
        var results = new[]
        {
            Result.Success(1),
            Result.Success(2),
            Result.Success(3)
        };

        var combined = results.Combine();

        combined.IsSuccess.Should().BeTrue();
        combined.Value.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void Combine_OneFailure_ReturnsFailure()
    {
        var results = new[]
        {
            Result.Success(1),
            Result.Failure<int>("fail"),
            Result.Success(3)
        };

        var combined = results.Combine();

        combined.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void NullableToResult_WithValue_ReturnsSuccess()
    {
        string? value = "hello";

        var result = value.ToResult("not found");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void NullableToResult_Null_ReturnsFailure()
    {
        string? value = null;

        var result = value.ToResult("not found");

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("not found");
    }
}
