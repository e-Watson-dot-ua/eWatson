using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class EmailAddressTests
{
    [Fact]
    public void Constructor_ValidInput_Creates()
    {
        var email = new EmailAddress("User.Name+tag@Example.com");

        email.Value.Should().Be("User.Name+tag@example.com");
    }

    [Fact]
    public void Constructor_NormalizesDomainOnly()
    {
        var email = new EmailAddress("TEST@EXAMPLE.COM");

        email.Value.Should().Be("TEST@example.com");
        email.ToString().Should().Be("TEST@example.com");
    }

    [Fact]
    public void Constructor_TrimsOuterWhitespace()
    {
        var email = new EmailAddress("  User@Example.com  ");

        email.Value.Should().Be("User@example.com");
    }

    [Fact]
    public void Constructor_InvalidFormat_Throws()
    {
        var act = () => new EmailAddress("invalid-email");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_NullInput_Throws()
    {
        var act = () => new EmailAddress(null!);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_TooLong_Throws()
    {
        var localPart = new string('a', 246);
        var act = () => new EmailAddress($"{localPart}@test.com");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void ImplicitConversion_ReturnsNormalizedValue()
    {
        var email = new EmailAddress("Hello@Example.com");

        string value = email;

        value.Should().Be("Hello@example.com");
    }
}
