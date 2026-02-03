using eWatson.Guards;
using eWatson.ValueObjects.Resources;
using System.Text.RegularExpressions;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a validated phone number value object.
/// </summary>
/// <remarks>
/// Phone numbers are stored in normalized format (digits only) and can be
/// formatted for display. Supports international phone numbers with optional
/// country code prefix (+).
/// </remarks>
public sealed partial class PhoneNumber : ValueObject
{
    private const int MinDigits = 7;
    private const int MaxDigits = 15; // E.164 standard

    [GeneratedRegex(@"^\+?\d+$")]
    private static partial Regex PhoneRegex();

    /// <summary>
    /// Gets the normalized phone number (digits only, no formatting).
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the country code if provided, otherwise null.
    /// </summary>
    public string? CountryCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> class.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the phone number is null, empty, contains invalid
    /// characters, or has an invalid length.
    /// </exception>
    public PhoneNumber(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);

        var normalized = NormalizePhoneNumber(value);

        Guard.Against.False(
            PhoneRegex().IsMatch(normalized),
            ValueObjectMessages.PhoneNumberInvalidCharacters());

        var digits = normalized.Replace("+", string.Empty);

        Guard.Against.True(
            digits.Length < MinDigits || digits.Length > MaxDigits,
            ValueObjectMessages.PhoneNumberInvalidLength(
                MinDigits,
                MaxDigits));

        if (normalized.StartsWith('+'))
        {
            CountryCode = ExtractCountryCode(digits);
            Value = digits;
        }
        else
        {
            CountryCode = null;
            Value = digits;
        }
    }

    /// <summary>
    /// Formats the phone number for display.
    /// </summary>
    /// <param name="format">
    /// The format to use. Options: "E164" (+1234567890),
    /// "International" (+1 234 567 890), "National" (234-567-890).
    /// Default is "E164".
    /// </param>
    /// <returns>The formatted phone number.</returns>
    public string Format(string format = "E164")
    {
        return format.ToUpperInvariant() switch
        {
            "E164" => CountryCode != null ? $"+{Value}" : Value,
            "INTERNATIONAL" => FormatInternational(),
            "NATIONAL" => FormatNational(),
            _ => Value
        };
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns a string representation of the phone number in E164 format.
    /// </summary>
    public override string ToString() => Format("E164");

    /// <summary>
    /// Implicitly converts a <see cref="PhoneNumber"/> to a string.
    /// </summary>
    /// <param name="phoneNumber">The phone number to convert.</param>
    public static implicit operator string(PhoneNumber phoneNumber)
        => phoneNumber.Value;

    private static string NormalizePhoneNumber(string value)
    {
        var normalized = new string(
            value.Where(c => char.IsDigit(c) || c == '+').ToArray());
        return normalized;
    }

    private static string ExtractCountryCode(string digits)
    {
        return digits.Length > 10 ? digits[..^10] : string.Empty;
    }

    private string FormatInternational()
    {
        if (CountryCode != null && Value.Length > 10)
        {
            var cc = Value[..^10];
            var number = Value[^10..];
            return $"+{cc} {FormatNumberWithSpaces(number)}";
        }

        return FormatNumberWithSpaces(Value);
    }

    private string FormatNational()
    {
        if (Value.Length == 10)
        {
            return $"{Value[..3]}-{Value[3..6]}-{Value[6..]}";
        }

        return Value;
    }

    private static string FormatNumberWithSpaces(string number)
    {
        if (number.Length <= 3)
        {
            return number;
        }

        var parts = new List<string>();
        for (int i = 0; i < number.Length; i += 3)
        {
            var length = Math.Min(3, number.Length - i);
            parts.Add(number.Substring(i, length));
        }

        return string.Join(" ", parts);
    }
}
