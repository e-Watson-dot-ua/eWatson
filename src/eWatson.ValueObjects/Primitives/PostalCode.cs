using eWatson.Guards;
using eWatson.ValueObjects.Resources;
using System.Text.RegularExpressions;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a validated postal code value object.
/// </summary>
/// <remarks>
/// Postal codes are validated based on country-specific formats. Currently
/// supports US (ZIP codes), CA (Canadian postal codes), UK, and a generic
/// alphanumeric format for other countries.
/// </remarks>
public sealed partial class PostalCode : ValueObject
{
    [GeneratedRegex(@"^\d{5}(-\d{4})?$")]
    private static partial Regex UsZipCodeRegex();

    [GeneratedRegex(
        @"^[A-Za-z]\d[A-Za-z]\s?\d[A-Za-z]\d$",
        RegexOptions.IgnoreCase)]
    private static partial Regex CanadianPostalCodeRegex();

    [GeneratedRegex(
        @"^[A-Z]{1,2}\d{1,2}[A-Z]?\s?\d[A-Z]{2}$",
        RegexOptions.IgnoreCase)]
    private static partial Regex UkPostalCodeRegex();

    [GeneratedRegex(@"^[A-Za-z0-9\s\-]{3,10}$")]
    private static partial Regex GenericPostalCodeRegex();

    /// <summary>
    /// Gets the postal code value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the country code for the postal code (e.g., US, CA, UK).
    /// </summary>
    public string CountryCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostalCode"/> class.
    /// </summary>
    /// <param name="value">The postal code string.</param>
    /// <param name="countryCode">
    /// The two-letter ISO country code (e.g., US, CA, UK, DE).
    /// Default is "US".
    /// </param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the postal code is null, empty, or not in a valid format
    /// for the specified country.
    /// </exception>
    public PostalCode(string value, string countryCode = "US")
    {
        Guard.Against.NullOrWhiteSpace(value);
        Guard.Against.NullOrWhiteSpace(countryCode);
        Guard.Against.True(
            countryCode.Length != 2,
            ValueObjectMessages.PostalCodeInvalidCountryCodeLength());

        var normalizedValue = value.Trim().ToUpperInvariant();
        var normalizedCountry = countryCode.ToUpperInvariant();

        Guard.Against.False(
            IsValidForCountry(normalizedValue, normalizedCountry),
            ValueObjectMessages.PostalCodeInvalidForCountry(
                value,
                countryCode));

        Value = normalizedValue;
        CountryCode = normalizedCountry;
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return CountryCode;
    }

    /// <summary>
    /// Returns a string representation of the postal code.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Implicitly converts a <see cref="PostalCode"/> to a string.
    /// </summary>
    /// <param name="postalCode">The postal code to convert.</param>
    public static implicit operator string(PostalCode postalCode)
        => postalCode.Value;

    private static bool IsValidForCountry(string value, string countryCode)
    {
        return countryCode switch
        {
            "US" => UsZipCodeRegex().IsMatch(value),
            "CA" => CanadianPostalCodeRegex().IsMatch(value),
            "UK" or "GB" => UkPostalCodeRegex().IsMatch(value),
            _ => GenericPostalCodeRegex().IsMatch(value)
        };
    }
}
