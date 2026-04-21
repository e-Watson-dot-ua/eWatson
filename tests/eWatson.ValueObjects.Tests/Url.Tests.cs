using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class UrlTests
{
    [Fact]
    public void Constructor_ValidAbsoluteUrl_Creates()
    {
        var url = new Url("https://example.com/orders?id=42");

        url.Scheme.Should().Be("https");
        url.Host.Should().Be("example.com");
        url.Path.Should().Be("/orders");
    }

    [Fact]
    public void Constructor_InvalidUrl_Throws()
    {
        var act = () => new Url("not a url");

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Constructor_RelativeUrl_WithRelativeKind_Creates()
    {
        var url = new Url("/orders/42", UriKind.Relative);

        url.Value.Should().Be("/orders/42");
        url.Uri.IsAbsoluteUri.Should().BeFalse();
    }

    [Fact]
    public void FromUri_CreatesUrl()
    {
        var uri = new Uri("https://example.com/catalog");

        var url = Url.FromUri(uri);

        url.Value.Should().Be("https://example.com/catalog");
        url.Uri.Should().Be(uri);
    }

    [Fact]
    public void FromUri_Null_Throws()
    {
        var act = () => Url.FromUri(null!);

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void ImplicitConversions_ReturnUriAndValue()
    {
        var url = new Url("https://example.com");

        string value = url;
        Uri uri = url;

        value.Should().Be("https://example.com/");
        uri.Should().Be(new Uri("https://example.com"));
    }
}
