using System.Globalization;

namespace eWatson.ValueObjects.Resources;

/// <summary>
/// Contains error messages for value object validations.
/// </summary>
internal static class ValueObjectMessages
{
    private static readonly System.Resources.ResourceManager Rm =
        new(typeof(ValueObjectMessages));

    public static string EmailAddressExceedsMaxLength(int maxLength)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("EmailAddressExceedsMaxLength", CultureInfo.InvariantCulture)!, maxLength);

    public static string EmailAddressInvalidFormat()
        => Rm.GetString("EmailAddressInvalidFormat", CultureInfo.InvariantCulture)!;

    public static string PhoneNumberInvalidCharacters()
        => Rm.GetString("PhoneNumberInvalidCharacters", CultureInfo.InvariantCulture)!;

    public static string PhoneNumberInvalidLength(int minDigits, int maxDigits)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("PhoneNumberInvalidLength", CultureInfo.InvariantCulture)!, minDigits, maxDigits);

    public static string MoneyInvalidCurrencyCodeLength()
        => Rm.GetString("MoneyInvalidCurrencyCodeLength", CultureInfo.InvariantCulture)!;

    public static string MoneyCannotAddDifferentCurrencies(string leftCurrency, string rightCurrency)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("MoneyCannotAddDifferentCurrencies", CultureInfo.InvariantCulture)!,
            leftCurrency, rightCurrency);

    public static string MoneyCannotSubtractDifferentCurrencies(string leftCurrency, string rightCurrency)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("MoneyCannotSubtractDifferentCurrencies", CultureInfo.InvariantCulture)!,
            leftCurrency, rightCurrency);

    public static string UrlInvalidFormat(string value)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("UrlInvalidFormat", CultureInfo.InvariantCulture)!, value);

    public static string PostalCodeInvalidCountryCodeLength()
        => Rm.GetString("PostalCodeInvalidCountryCodeLength", CultureInfo.InvariantCulture)!;

    public static string PostalCodeInvalidForCountry(string value, string countryCode)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("PostalCodeInvalidForCountry", CultureInfo.InvariantCulture)!, value, countryCode);

    public static string DateRangeStartMustBeBeforeEnd(DateOnly start, DateOnly end)
        => string.Format(CultureInfo.InvariantCulture,
            Rm.GetString("DateRangeStartMustBeBeforeEnd", CultureInfo.InvariantCulture)!, start, end);
}
