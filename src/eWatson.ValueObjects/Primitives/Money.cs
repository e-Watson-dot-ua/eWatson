using eWatson.Guards;
using eWatson.ValueObjects.Resources;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a monetary amount with a currency code.
/// </summary>
/// <remarks>
/// Money value objects include both an amount and a three-letter ISO 4217
/// currency code (e.g., USD, EUR, GBP). Arithmetic operations between Money
/// instances require matching currencies.
/// </remarks>
public sealed class Money : ValueObject
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the three-letter ISO 4217 currency code (e.g., USD, EUR, GBP).
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Money"/> class.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">
    /// The three-letter ISO 4217 currency code (e.g., USD, EUR, GBP).
    /// </param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the currency is null, empty, or not exactly 3 characters.
    /// </exception>
    public Money(decimal amount, string currency)
    {
        Guard.Against.NullOrWhiteSpace(currency);
        Guard.Against.True(
            currency.Length != 3,
            ValueObjectMessages.MoneyInvalidCurrencyCodeLength());

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a zero money instance with the specified currency.
    /// </summary>
    /// <param name="currency">
    /// The three-letter ISO 4217 currency code.
    /// </param>
    /// <returns>A Money instance with amount 0.</returns>
    public static Money Zero(string currency) => new(0, currency);

    /// <summary>
    /// Adds two money instances.
    /// </summary>
    /// <param name="left">The first money instance.</param>
    /// <param name="right">The second money instance.</param>
    /// <returns>A new Money instance with the sum of the amounts.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the currencies do not match.
    /// </exception>
    public static Money operator +(Money left, Money right)
    {
        Guard.Against.True(
            left.Currency != right.Currency,
            ValueObjectMessages.MoneyCannotAddDifferentCurrencies(
                left.Currency,
                right.Currency));

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts one money instance from another.
    /// </summary>
    /// <param name="left">The money instance to subtract from.</param>
    /// <param name="right">The money instance to subtract.</param>
    /// <returns>A new Money instance with the difference of the amounts.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the currencies do not match.
    /// </exception>
    public static Money operator -(Money left, Money right)
    {
        Guard.Against.True(
            left.Currency != right.Currency,
            ValueObjectMessages.MoneyCannotSubtractDifferentCurrencies(
                left.Currency,
                right.Currency));

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies a money instance by a scalar value.
    /// </summary>
    /// <param name="money">The money instance to multiply.</param>
    /// <param name="multiplier">The multiplier.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    /// <summary>
    /// Multiplies a money instance by a scalar value.
    /// </summary>
    /// <param name="multiplier">The multiplier.</param>
    /// <param name="money">The money instance to multiply.</param>
    /// <returns>A new Money instance with the multiplied amount.</returns>
    public static Money operator *(decimal multiplier, Money money)
    {
        return money * multiplier;
    }

    /// <summary>
    /// Divides a money instance by a scalar value.
    /// </summary>
    /// <param name="money">The money instance to divide.</param>
    /// <param name="divisor">The divisor.</param>
    /// <returns>A new Money instance with the divided amount.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the divisor is zero.
    /// </exception>
    public static Money operator /(Money money, decimal divisor)
    {
        Guard.Against.InvalidValue(
            divisor,
            0m,
            nameof(divisor));

        return new Money(money.Amount / divisor, money.Currency);
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <summary>
    /// Returns a string representation of the money value.
    /// </summary>
    public override string ToString() => $"{Amount:N2} {Currency}";
}
