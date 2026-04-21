using eWatson.Guards.Exceptions;
using eWatson.ValueObjects.Primitives;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class DateRangeTests
{
    [Fact]
    public void Constructor_ValidRange_Creates()
    {
        var range = new DateRange(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 3));

        range.Start.Should().Be(new DateOnly(2026, 4, 1));
        range.End.Should().Be(new DateOnly(2026, 4, 3));
        range.DurationInDays.Should().Be(3);
    }

    [Fact]
    public void Constructor_StartAfterEnd_Throws()
    {
        var act = () => new DateRange(
            new DateOnly(2026, 4, 3),
            new DateOnly(2026, 4, 1));

        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void SingleDay_CreatesOneDayRange()
    {
        var range = DateRange.SingleDay(new DateOnly(2026, 4, 21));

        range.Start.Should().Be(new DateOnly(2026, 4, 21));
        range.End.Should().Be(new DateOnly(2026, 4, 21));
        range.DurationInDays.Should().Be(1);
    }

    [Fact]
    public void Contains_InclusiveBounds_ReturnsTrue()
    {
        var range = new DateRange(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 3));

        range.Contains(new DateOnly(2026, 4, 1)).Should().BeTrue();
        range.Contains(new DateOnly(2026, 4, 3)).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_IntersectingRanges_ReturnsTrue()
    {
        var left = new DateRange(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 3));
        var right = new DateRange(
            new DateOnly(2026, 4, 3),
            new DateOnly(2026, 4, 5));

        left.Overlaps(right).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_NullRange_Throws()
    {
        var range = new DateRange(
            new DateOnly(2026, 4, 1),
            new DateOnly(2026, 4, 3));

        var act = () => range.Overlaps(null!);

        act.Should().Throw<GuardException>();
    }
}
