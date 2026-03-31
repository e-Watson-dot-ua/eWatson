using eWatson.Guards;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a percentage value between 0 and 100.
/// </summary>
/// <remarks>
/// Percentage values are stored as decimals and must be within the range
/// [0, 100]. Provides methods to convert between percentage and decimal
/// representations (e.g., 25% = 0.25).
/// </remarks>
public sealed class Percentage : ValueObject
{
    private const decimal MinValue = 0;
    private const decimal MaxValue = 100;

    /// <summary>
    /// Gets the percentage value (0-100).
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Percentage"/> class.
    /// </summary>
    /// <param name="value">The percentage value (0-100).</param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the value is not between 0 and 100 (inclusive).
    /// </exception>
    public Percentage(decimal value)
    {
        Guard.Against.OutOfRange(value, MinValue, MaxValue);
        Value = value;
    }

    /// <summary>
    /// Creates a Percentage from a decimal fraction (e.g., 0.25 = 25%).
    /// </summary>
    /// <param name="fraction">The decimal fraction (0-1).</param>
    /// <returns>A Percentage instance.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the fraction is not between 0 and 1 (inclusive).
    /// </exception>
    public static Percentage FromFraction(decimal fraction)
    {
        Guard.Against.OutOfRange(fraction, 0m, 1m);
        return new Percentage(fraction * 100);
    }

    /// <summary>
    /// Converts the percentage to a decimal fraction (e.g., 25% = 0.25).
    /// </summary>
    /// <returns>The decimal fraction representation.</returns>
    public decimal ToFraction() => Value / 100;

    /// <summary>
    /// Calculates the percentage of a given amount.
    /// </summary>
    /// <param name="amount">The amount to calculate the percentage of.</param>
    /// <returns>The calculated percentage value.</returns>
    public decimal Of(decimal amount) => amount * ToFraction();

    /// <summary>
    /// Adds two percentages.
    /// </summary>
    /// <param name="left">The first percentage.</param>
    /// <param name="right">The second percentage.</param>
    /// <returns>A new Percentage with the sum of the values.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the sum exceeds 100.
    /// </exception>
    public static Percentage operator +(Percentage left, Percentage right)
    {
        return new Percentage(checked(left.Value + right.Value));
    }

    /// <summary>
    /// Subtracts one percentage from another.
    /// </summary>
    /// <param name="left">The percentage to subtract from.</param>
    /// <param name="right">The percentage to subtract.</param>
    /// <returns>A new Percentage with the difference of the values.</returns>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the difference is less than 0.
    /// </exception>
    public static Percentage operator -(Percentage left, Percentage right)
    {
        return new Percentage(checked(left.Value - right.Value));
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns a string representation of the percentage.
    /// </summary>
    public override string ToString() => $"{Value}%";

    /// <summary>
    /// Implicitly converts a <see cref="Percentage"/> to a decimal.
    /// </summary>
    /// <param name="percentage">The percentage to convert.</param>
    public static implicit operator decimal(Percentage percentage)
        => percentage.Value;
}
