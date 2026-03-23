using eWatson.Primitives.Maybes;
using eWatson.Primitives.Results;
using FluentAssertions;

namespace eWatson.Primitives.Tests;

public sealed class MaybeExtensionsTests
{
    [Fact]
    public void Match_Some_CallsOnSome()
    {
        var maybe = Maybe.Some(5);

        var result = maybe.Match(v => v * 2, () => -1);

        result.Should().Be(10);
    }

    [Fact]
    public void Match_None_CallsOnNone()
    {
        var maybe = Maybe.None<int>();

        var result = maybe.Match(v => v * 2, () => -1);

        result.Should().Be(-1);
    }

    [Fact]
    public void Map_Some_Transforms()
    {
        var maybe = Maybe.Some(3);

        var mapped = maybe.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.HasValue.Should().BeTrue();
        mapped.Value.Should().Be("3");
    }

    [Fact]
    public void Map_None_ReturnsNone()
    {
        var maybe = Maybe.None<int>();

        var mapped = maybe.Map(v => v.ToString(System.Globalization.CultureInfo.InvariantCulture));

        mapped.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void Bind_Some_Chains()
    {
        var maybe = Maybe.Some(5);

        var bound = maybe.Bind(v => v > 3 ? Maybe.Some("big") : Maybe.None<string>());

        bound.HasValue.Should().BeTrue();
        bound.Value.Should().Be("big");
    }

    [Fact]
    public void Bind_None_ReturnsNone()
    {
        var maybe = Maybe.None<int>();

        var bound = maybe.Bind(v => Maybe.Some("x"));

        bound.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void OrElse_Some_ReturnsValue()
    {
        var maybe = Maybe.Some(42);

        maybe.OrElse(0).Should().Be(42);
    }

    [Fact]
    public void OrElse_None_ReturnsFallback()
    {
        var maybe = Maybe.None<int>();

        maybe.OrElse(99).Should().Be(99);
    }

    [Fact]
    public void OrElse_Factory_None_InvokesFactory()
    {
        var maybe = Maybe.None<int>();

        maybe.OrElse(() => 99).Should().Be(99);
    }

    [Fact]
    public void OrDefault_Some_ReturnsValue()
    {
        var maybe = Maybe.Some(7);

        maybe.OrDefault().Should().Be(7);
    }

    [Fact]
    public void OrDefault_None_ReturnsDefault()
    {
        var maybe = Maybe.None<int>();

        maybe.OrDefault().Should().Be(0);
    }

    [Fact]
    public void Where_MatchingPredicate_ReturnsSame()
    {
        var maybe = Maybe.Some(10);

        var filtered = maybe.Where(v => v > 5);

        filtered.HasValue.Should().BeTrue();
    }

    [Fact]
    public void Where_NonMatchingPredicate_ReturnsNone()
    {
        var maybe = Maybe.Some(2);

        var filtered = maybe.Where(v => v > 5);

        filtered.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void ToResult_Some_ReturnsSuccess()
    {
        var maybe = Maybe.Some("hello");

        var result = maybe.ToResult("not found");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ToResult_None_ReturnsFailure()
    {
        var maybe = Maybe.None<string>();

        var result = maybe.ToResult("not found");

        result.IsFailure.Should().BeTrue();
        result.ErrorMessage.Should().Be("not found");
    }

    [Fact]
    public void Tap_Some_ExecutesAction()
    {
        var tapped = false;
        var maybe = Maybe.Some(1);

        var returned = maybe.Tap(_ => tapped = true);

        tapped.Should().BeTrue();
        returned.Should().Be(maybe);
    }

    [Fact]
    public void Tap_None_DoesNotExecuteAction()
    {
        var tapped = false;
        var maybe = Maybe.None<int>();

        maybe.Tap(_ => tapped = true);

        tapped.Should().BeFalse();
    }

    [Fact]
    public void ToMaybe_NonNullReference_ReturnsSome()
    {
        string? value = "hello";

        var maybe = value.ToMaybe();

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be("hello");
    }

    [Fact]
    public void ToMaybe_NullReference_ReturnsNone()
    {
        string? value = null;

        var maybe = value.ToMaybe();

        maybe.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public void ToMaybe_NullableStruct_WithValue_ReturnsSome()
    {
        int? value = 42;

        var maybe = value.ToMaybe();

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be(42);
    }

    [Fact]
    public void ToMaybe_NullableStruct_Null_ReturnsNone()
    {
        int? value = null;

        var maybe = value.ToMaybe();

        maybe.HasNoValue.Should().BeTrue();
    }
}
