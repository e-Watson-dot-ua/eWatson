namespace eWatson.ValueObjects.Resources;

/// <summary>
/// Contains error messages for value object validations.
/// </summary>
internal static class ValueObjectMessages
{
    private static readonly System.Resources.ResourceManager _rm =
        new("eWatson.ValueObjects.Resources.ValueObjectMessages",
            typeof(ValueObjectMessages).Assembly);

    public static string EmailAddressExceedsMaxLength(int maxLength)
        => string.Format(
            _rm.GetString("EmailAddressExceedsMaxLength")!,
            maxLength);

    public static string EmailAddressInvalidFormat()
        => _rm.GetString("EmailAddressInvalidFormat")!;

    public static string PhoneNumberInvalidCharacters()
        => _rm.GetString("PhoneNumberInvalidCharacters")!;

    public static string PhoneNumberInvalidLength(int minDigits, int maxDigits)
        => string.Format(
            _rm.GetString("PhoneNumberInvalidLength")!,
            minDigits,
            maxDigits);

    public static string MoneyInvalidCurrencyCodeLength()
        => _rm.GetString("MoneyInvalidCurrencyCodeLength")!;

    public static string MoneyCannotAddDifferentCurrencies(
        string leftCurrency,
        string rightCurrency)
        => string.Format(
            _rm.GetString("MoneyCannotAddDifferentCurrencies")!,
            leftCurrency,
            rightCurrency);

    public static string MoneyCannotSubtractDifferentCurrencies(
        string leftCurrency,
        string rightCurrency)
        => string.Format(
            _rm.GetString("MoneyCannotSubtractDifferentCurrencies")!,
            leftCurrency,
            rightCurrency);

    public static string UrlInvalidFormat(string value)
        => string.Format(_rm.GetString("UrlInvalidFormat")!, value);

    public static string PostalCodeInvalidCountryCodeLength()
        => _rm.GetString("PostalCodeInvalidCountryCodeLength")!;

    public static string PostalCodeInvalidForCountry(
        string value,
        string countryCode)
        => string.Format(
            _rm.GetString("PostalCodeInvalidForCountry")!,
            value,
            countryCode);

    public static string DateRangeStartMustBeBeforeEnd(
        DateOnly start,
        DateOnly end)
        => string.Format(
            _rm.GetString("DateRangeStartMustBeBeforeEnd")!,
            start,
            end);
}
