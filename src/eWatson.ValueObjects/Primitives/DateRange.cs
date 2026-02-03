using eWatson.Guards;
using eWatson.ValueObjects.Resources;

namespace eWatson.ValueObjects.Primitives;

/// <summary>
/// Represents a date range with start and end dates.
/// </summary>
/// <remarks>
/// A date range consists of a start date and an end date, where the start
/// date must be less than or equal to the end date. Provides methods to
/// check if a date falls within the range and calculate the duration.
/// </remarks>
public sealed class DateRange : ValueObject
{
    /// <summary>
    /// Gets the start date of the range (inclusive).
    /// </summary>
    public DateOnly Start { get; }

    /// <summary>
    /// Gets the end date of the range (inclusive).
    /// </summary>
    public DateOnly End { get; }

    /// <summary>
    /// Gets the number of days in the date range (inclusive).
    /// </summary>
    public int DurationInDays => End.DayNumber - Start.DayNumber + 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateRange"/> class.
    /// </summary>
    /// <param name="start">The start date of the range.</param>
    /// <param name="end">The end date of the range.</param>
    /// <exception cref="Guards.Exceptions.GuardException">
    /// Thrown when the start date is after the end date.
    /// </exception>
    public DateRange(DateOnly start, DateOnly end)
    {
        Guard.Against.True(
            start > end,
            ValueObjectMessages.DateRangeStartMustBeBeforeEnd(start, end));

        Start = start;
        End = end;
    }

    /// <summary>
    /// Creates a date range from DateTime values.
    /// </summary>
    /// <param name="start">The start date and time.</param>
    /// <param name="end">The end date and time.</param>
    /// <returns>A new DateRange instance.</returns>
    public static DateRange FromDateTime(DateTime start, DateTime end)
    {
        return new DateRange(
            DateOnly.FromDateTime(start),
            DateOnly.FromDateTime(end));
    }

    /// <summary>
    /// Creates a date range for a single day.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>A new DateRange instance with start and end on the same day.</returns>
    public static DateRange SingleDay(DateOnly date)
    {
        return new DateRange(date, date);
    }

    /// <summary>
    /// Determines if a date falls within this date range (inclusive).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>
    /// <c>true</c> if the date is within the range; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(DateOnly date)
    {
        return date >= Start && date <= End;
    }

    /// <summary>
    /// Determines if this date range overlaps with another date range.
    /// </summary>
    /// <param name="other">The other date range to check.</param>
    /// <returns>
    /// <c>true</c> if the ranges overlap; otherwise, <c>false</c>.
    /// </returns>
    public bool Overlaps(DateRange other)
    {
        Guard.Against.Null(other);
        return Start <= other.End && End >= other.Start;
    }

    /// <summary>
    /// Returns the equality components for this value object.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    /// <summary>
    /// Returns a string representation of the date range.
    /// </summary>
    public override string ToString()
        => $"{Start:yyyy-MM-dd} to {End:yyyy-MM-dd}";
}
