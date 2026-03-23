using eWatson.Primitives.Maybes;
using FluentAssertions;

namespace eWatson.Primitives.Tests;

public sealed class MaybeTests
{
    [Fact]
    public void Some_WithValue_HasValue()
    {
        var maybe = Maybe.Some("hello");

        maybe.HasValue.Should().BeTrue();
        maybe.HasNoValue.Should().BeFalse();
        maybe.Value.Should().Be("hello");
    }

    [Fact]
    public void None_HasNoValue()
    {
        var maybe = Maybe.None<string>();

        maybe.HasNoValue.Should().BeTrue();
        maybe.HasValue.Should().BeFalse();
    }

    [Fact]
    public void None_Value_Throws()
    {
        var maybe = Maybe.None<string>();

        var act = () => maybe.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Some_Null_Throws()
    {
        var act = () => Maybe.Some<string>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSome()
    {
        Maybe<int> maybe = 42;

        maybe.HasValue.Should().BeTrue();
        maybe.Value.Should().Be(42);
    }

    [Fact]
    public void Equals_TwoSomeWithSameValue_AreEqual()
    {
        var a = Maybe.Some(5);
        var b = Maybe.Some(5);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoNone_AreEqual()
    {
        var a = Maybe.None<int>();
        var b = Maybe.None<int>();

        a.Should().Be(b);
    }

    [Fact]
    public void Equals_SomeAndNone_AreNotEqual()
    {
        var some = Maybe.Some(1);
        var none = Maybe.None<int>();

        some.Should().NotBe(none);
        (some != none).Should().BeTrue();
    }

    [Fact]
    public void ToString_Some_ReturnsValueString()
    {
        var maybe = Maybe.Some(42);
        maybe.ToString().Should().Be("42");
    }

    [Fact]
    public void ToString_None_ReturnsNone()
    {
        var maybe = Maybe.None<int>();
        maybe.ToString().Should().Be("None");
    }

    [Fact]
    public void GetHashCode_SameValues_AreSame()
    {
        var a = Maybe.Some("test");
        var b = Maybe.Some("test");

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_None_ReturnsZero()
    {
        Maybe.None<string>().GetHashCode().Should().Be(0);
    }
}
