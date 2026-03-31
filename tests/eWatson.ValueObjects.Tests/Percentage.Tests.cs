using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class PercentageTests
{
    [Fact]
    public void Constructor_ValidValue_Creates()
    {
        var pct = new Percentage(50m);

        pct.Value.Should().Be(50m);
    }

    [Fact]
    public void Constructor_Zero_Creates()
    {
        var pct = new Percentage(0m);

        pct.Value.Should().Be(0m);
    }

    [Fact]
    public void Constructor_Hundred_Creates()
    {
        var pct = new Percentage(100m);

        pct.Value.Should().Be(100m);
    }

    [Fact]
    public void Constructor_BelowZero_Throws()
    {
        var act = () => new Percentage(-1m);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_AboveHundred_Throws()
    {
        var act = () => new Percentage(101m);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void FromFraction_ValidFraction_Creates()
    {
        var pct = Percentage.FromFraction(0.25m);

        pct.Value.Should().Be(25m);
    }

    [Fact]
    public void FromFraction_OutOfRange_Throws()
    {
        var act = () => Percentage.FromFraction(1.5m);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void ToFraction_ReturnsCorrectValue()
    {
        var pct = new Percentage(25m);

        pct.ToFraction().Should().Be(0.25m);
    }

    [Fact]
    public void Of_CalculatesCorrectly()
    {
        var pct = new Percentage(10m);

        pct.Of(200m).Should().Be(20m);
    }

    [Fact]
    public void Addition_WithinRange_Succeeds()
    {
        var a = new Percentage(40m);
        var b = new Percentage(30m);

        var result = a + b;

        result.Value.Should().Be(70m);
    }

    [Fact]
    public void Addition_ExceedsHundred_Throws()
    {
        var a = new Percentage(60m);
        var b = new Percentage(50m);

        var act = () => a + b;

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Subtraction_WithinRange_Succeeds()
    {
        var a = new Percentage(70m);
        var b = new Percentage(30m);

        var result = a - b;

        result.Value.Should().Be(40m);
    }

    [Fact]
    public void Subtraction_BelowZero_Throws()
    {
        var a = new Percentage(30m);
        var b = new Percentage(50m);

        var act = () => a - b;

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Equality_SameValue_Equal()
    {
        var a = new Percentage(50m);
        var b = new Percentage(50m);

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentValue_NotEqual()
    {
        var a = new Percentage(50m);
        var b = new Percentage(60m);

        a.Should().NotBe(b);
    }

    [Fact]
    public void ImplicitConversion_ToDecimal()
    {
        var pct = new Percentage(75m);
        decimal value = pct;

        value.Should().Be(75m);
    }

    [Fact]
    public void ToString_ContainsPercentSign()
    {
        var pct = new Percentage(42.5m);

        pct.ToString().Should().EndWith("%");
        pct.ToString().Should().Contain("42");
    }
}
