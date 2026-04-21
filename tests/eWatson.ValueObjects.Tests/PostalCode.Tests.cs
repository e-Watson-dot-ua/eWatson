using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class PostalCodeTests
{
    [Fact]
    public void Constructor_ValidUsPostalCode_Creates()
    {
        var postalCode = new PostalCode("12345");

        postalCode.Value.Should().Be("12345");
        postalCode.CountryCode.Should().Be("US");
    }

    [Fact]
    public void Constructor_NormalizesValueAndCountryCode()
    {
        var postalCode = new PostalCode("k1a 0b1", "ca");

        postalCode.Value.Should().Be("K1A 0B1");
        postalCode.CountryCode.Should().Be("CA");
        postalCode.ToString().Should().Be("K1A 0B1");
    }

    [Fact]
    public void Constructor_InvalidFormatForCountry_Throws()
    {
        var act = () => new PostalCode("ABCDE", "US");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_InvalidCountryCodeLength_Throws()
    {
        var act = () => new PostalCode("12345", "USA");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_GenericCountry_UsesFallbackPattern()
    {
        var postalCode = new PostalCode("10-101", "PL");

        postalCode.Value.Should().Be("10-101");
        postalCode.CountryCode.Should().Be("PL");
    }

    [Fact]
    public void ImplicitConversion_ReturnsNormalizedValue()
    {
        var postalCode = new PostalCode("12345-6789");

        string value = postalCode;

        value.Should().Be("12345-6789");
    }
}
