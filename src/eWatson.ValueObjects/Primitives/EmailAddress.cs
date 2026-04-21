using eWatson.Guards;
using eWatson.ValueObjects.Resources;
using System.Text.RegularExpressions;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a validated email address value object.
/// </summary>
/// <remarks>
/// Email addresses are validated using a regex pattern that checks for basic
/// structure (local@domain). This is a pragmatic validator rather than a full
/// RFC implementation. The original local part is preserved, while the domain
/// part is normalized to lowercase for stable comparisons.
/// </remarks>
public sealed partial class EmailAddress : ValueObject
{
    private const int MaxLength = 254; // RFC 5321
    private const string EmailPattern =
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    [GeneratedRegex(EmailPattern, RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    /// <summary>
    /// Gets the email address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAddress"/> class.
    /// </summary>
    /// <param name="value">The email address string.</param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the email address is null, empty, exceeds maximum length,
    /// or is not in a valid format.
    /// </exception>
    public EmailAddress(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        value = value.Trim();
        Guard.Against.True(
            value.Length > MaxLength,
            ValueObjectMessages.EmailAddressExceedsMaxLength(MaxLength));
        Guard.Against.False(
            EmailRegex().IsMatch(value),
            ValueObjectMessages.EmailAddressInvalidFormat());

        var separatorIndex = value.LastIndexOf('@');
        var localPart = value[..separatorIndex];
        var domainPart = value[(separatorIndex + 1)..];

        Value = $"{localPart}@{domainPart.ToLowerInvariant()}";
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns a string representation of the email address.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts an <see cref="EmailAddress"/> to a string.
    /// </summary>
    /// <param name="emailAddress">The email address to convert.</param>
    public static implicit operator string(EmailAddress emailAddress)
        => emailAddress.Value;
}
