using eWatson.Guards;
using eWatson.ValueObjects.Resources;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a validated URL value object.
/// </summary>
/// <remarks>
/// URL validation uses the built-in Uri class to ensure the URL is
/// well-formed. Supports both absolute and relative URLs based on the
/// UriKind parameter.
/// </remarks>
public sealed class Url : ValueObject
{
    /// <summary>
    /// Gets the URL value as a string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the Uri instance representing this URL.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    /// Gets the scheme (protocol) of the URL (e.g., http, https, ftp).
    /// </summary>
    public string Scheme => Uri.Scheme;

    /// <summary>
    /// Gets the host component of the URL.
    /// </summary>
    public string Host => Uri.Host;

    /// <summary>
    /// Gets the path component of the URL.
    /// </summary>
    public string Path => Uri.AbsolutePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="Url"/> class.
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <param name="uriKind">
    /// The kind of URI (Absolute, Relative, or RelativeOrAbsolute).
    /// Default is Absolute.
    /// </param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the URL is null, empty, or not in a valid format.
    /// </exception>
    public Url(string value, UriKind uriKind = UriKind.Absolute)
    {
        Guard.Against.NullOrWhiteSpace(value);
        Guard.Against.False(
            Uri.TryCreate(value, uriKind, out var uri),
            ValueObjectMessages.UrlInvalidFormat(value));

        Uri = uri!;
        Value = Uri.ToString();
    }

    /// <summary>
    /// Creates a URL from a Uri instance.
    /// </summary>
    /// <param name="uri">The Uri instance.</param>
    /// <returns>A new Url value object.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the uri is null.
    /// </exception>
    public static Url FromUri(Uri uri)
    {
        Guard.Against.Null(uri);
        return new Url(uri.ToString());
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns a string representation of the URL.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="Url"/> to a string.
    /// </summary>
    /// <param name="url">The URL to convert.</param>
    public static implicit operator string(Url url) => url.Value;

    /// <summary>
    /// Implicitly converts a <see cref="Url"/> to a Uri.
    /// </summary>
    /// <param name="url">The URL to convert.</param>
    public static implicit operator Uri(Url url) => url.Uri;
}
