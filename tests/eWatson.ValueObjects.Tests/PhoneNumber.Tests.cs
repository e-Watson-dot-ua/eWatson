using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class PhoneNumberTests
{
    [Fact]
    public void ValidLocalNumber_CreatesSuccessfully()
    {
        var phone = new PhoneNumber("2025551234");

        phone.Value.Should().Be("2025551234");
        phone.IsInternational.Should().BeFalse();
        phone.CountryCode.Should().BeNull();
    }

    [Fact]
    public void ValidInternationalNumber_CreatesSuccessfully()
    {
        var phone = new PhoneNumber("+12025551234");

        phone.Value.Should().Be("12025551234");
        phone.IsInternational.Should().BeTrue();
    }

    [Fact]
    public void NormalizesFormattedInput()
    {
        var phone = new PhoneNumber("+1 (202) 555-1234");

        phone.Value.Should().Be("12025551234");
        phone.IsInternational.Should().BeTrue();
    }

    [Fact]
    public void ShortInternationalNumber_CreatesSuccessfully()
    {
        // 7 digits is the minimum
        var phone = new PhoneNumber("+1234567");

        phone.Value.Should().Be("1234567");
        phone.IsInternational.Should().BeTrue();
    }

    [Fact]
    public void TooFewDigits_Throws()
    {
        var act = () => new PhoneNumber("123456");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void TooManyDigits_Throws()
    {
        var act = () => new PhoneNumber("1234567890123456"); // 16 digits

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullInput_Throws()
    {
        var act = () => new PhoneNumber(null!);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void EmptyInput_Throws()
    {
        var act = () => new PhoneNumber("");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void FormatE164_International()
    {
        var phone = new PhoneNumber("+12025551234");

        phone.Format("E164").Should().Be("+12025551234");
    }

    [Fact]
    public void FormatE164_Local()
    {
        var phone = new PhoneNumber("2025551234");

        phone.Format("E164").Should().Be("2025551234");
    }

    [Fact]
    public void FormatNational_10Digits()
    {
        var phone = new PhoneNumber("2025551234");

        phone.Format("National").Should().Be("202-555-1234");
    }

    [Fact]
    public void Equality_SameDigitsDifferentPrefix_NotEqual()
    {
        var local = new PhoneNumber("2025551234");
        var international = new PhoneNumber("+2025551234");

        local.Should().NotBe(international);
    }

    [Fact]
    public void Equality_SameDigitsSamePrefix_Equal()
    {
        var a = new PhoneNumber("+1 202 555 1234");
        var b = new PhoneNumber("+12025551234");

        a.Should().Be(b);
    }

    [Fact]
    public void ImplicitConversion_ReturnsValue()
    {
        var phone = new PhoneNumber("2025551234");
        string value = phone;

        value.Should().Be("2025551234");
    }
}
