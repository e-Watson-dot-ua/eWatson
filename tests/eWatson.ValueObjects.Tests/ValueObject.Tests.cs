using eWatson.Guards.Exceptions;
using eWatson.ValueObjects;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class ValueObjectEqualityTests
{
    private sealed class TestValueObject(string a, int b) : ValueObject
    {
        public string A { get; } = a;
        public int B { get; } = b;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return A;
            yield return B;
        }
    }

    [Fact]
    public void SameComponents_AreEqual()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("x", 1);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void DifferentComponents_AreNotEqual()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("y", 1);

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Null_IsNotEqual()
    {
        var a = new TestValueObject("x", 1);

        a.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameComponents_SameHash()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("x", 1);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}

public sealed class EmailAddressTests
{
    [Fact]
    public void ValidEmail_CreatesSuccessfully()
    {
        var email = new EmailAddress("Test@Example.COM");

        email.Value.Should().Be("test@example.com");
        email.ToString().Should().Be("test@example.com");
    }

    [Fact]
    public void InvalidEmail_Throws()
    {
        var act = () => new EmailAddress("not-an-email");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullEmail_Throws()
    {
        var act = () => new EmailAddress(null!);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void TooLongEmail_Throws()
    {
        var longLocal = new string('a', 250);
        var act = () => new EmailAddress($"{longLocal}@x.co");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void ImplicitConversion_ToString()
    {
        var email = new EmailAddress("user@test.com");
        string value = email;

        value.Should().Be("user@test.com");
    }

    [Fact]
    public void Value_IsPublic()
    {
        var email = new EmailAddress("user@test.com");

        // Verifies Value is publicly accessible (was previously private)
        email.Value.Should().NotBeNullOrEmpty();
    }
}

public sealed class UrlTests
{
    [Fact]
    public void ValidUrl_CreatesSuccessfully()
    {
        var url = new Url("https://example.com/path");

        url.Scheme.Should().Be("https");
        url.Host.Should().Be("example.com");
    }

    [Fact]
    public void InvalidUrl_Throws()
    {
        var act = () => new Url("not a url");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Equality_NormalizedUrls_AreEqual()
    {
        // Uri.ToString() normalizes the URL
        var a = new Url("HTTPS://EXAMPLE.COM/path");
        var b = new Url("https://example.com/path");

        a.Should().Be(b);
    }

    [Fact]
    public void FromUri_CreatesUrl()
    {
        var uri = new Uri("https://example.com");
        var url = Url.FromUri(uri);

        url.Uri.Should().Be(uri);
    }

    [Fact]
    public void ImplicitConversion_ToUri()
    {
        var url = new Url("https://example.com");
        Uri uri = url;

        uri.Should().NotBeNull();
    }
}
