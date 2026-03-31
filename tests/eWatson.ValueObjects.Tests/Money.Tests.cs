using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Constructor_ValidInput_Creates()
    {
        var money = new Money(100.50m, "USD");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_NormalizeCurrency_ToUpperCase()
    {
        var money = new Money(10m, "usd");

        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Constructor_NullCurrency_Throws()
    {
        var act = () => new Money(10m, null!);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_InvalidCurrencyLength_Throws()
    {
        var act = () => new Money(10m, "US");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Zero_CreatesZeroAmount()
    {
        var money = Money.Zero("EUR");

        money.Amount.Should().Be(0m);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Addition_SameCurrency_Succeeds()
    {
        var a = new Money(10m, "USD");
        var b = new Money(20m, "USD");

        var result = a + b;

        result.Amount.Should().Be(30m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_DifferentCurrency_Throws()
    {
        var a = new Money(10m, "USD");
        var b = new Money(20m, "EUR");

        var act = () => a + b;

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Subtraction_SameCurrency_Succeeds()
    {
        var a = new Money(30m, "USD");
        var b = new Money(10m, "USD");

        var result = a - b;

        result.Amount.Should().Be(20m);
    }

    [Fact]
    public void Multiplication_Succeeds()
    {
        var money = new Money(10m, "USD");

        var result = money * 3m;

        result.Amount.Should().Be(30m);
    }

    [Fact]
    public void Division_Succeeds()
    {
        var money = new Money(30m, "USD");

        var result = money / 3m;

        result.Amount.Should().Be(10m);
    }

    [Fact]
    public void Division_ByZero_Throws()
    {
        var money = new Money(10m, "USD");

        var act = () => money / 0m;

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_Equal()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "USD");

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentAmount_NotEqual()
    {
        var a = new Money(10m, "USD");
        var b = new Money(20m, "USD");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentCurrency_NotEqual()
    {
        var a = new Money(10m, "USD");
        var b = new Money(10m, "EUR");

        a.Should().NotBe(b);
    }

    [Fact]
    public void ToString_ContainsCurrencyCode()
    {
        var money = new Money(10.50m, "GBP");

        money.ToString().Should().Contain("GBP");
        money.ToString().Should().Contain("10");
    }
}
