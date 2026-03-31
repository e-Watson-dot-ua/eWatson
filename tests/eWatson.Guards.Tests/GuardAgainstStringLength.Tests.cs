using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstStringLengthTests
{
    #region StringTooLong Tests

    [Fact]
    public void StringTooLong_WithNull_ThrowsGuardException()
    {
        string? nullString = null;

        Action act = () => Guard.Against.StringTooLong(nullString!, 10);

        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*");
    }

    [Fact]
    public void StringTooLong_WithStringExceedingMax_ThrowsGuardException()
    {
        string longString = "this is way too long";

        Action act = () => Guard.Against.StringTooLong(longString, 5);

        act.Should().Throw<GuardException>()
            .WithMessage("*cannot exceed*");
    }

    [Fact]
    public void StringTooLong_WithStringAtMax_DoesNotThrow()
    {
        string exactString = "12345";

        Action act = () => Guard.Against.StringTooLong(exactString, 5);

        act.Should().NotThrow();
    }

    [Fact]
    public void StringTooLong_WithStringBelowMax_DoesNotThrow()
    {
        string shortString = "abc";

        Action act = () => Guard.Against.StringTooLong(shortString, 10);

        act.Should().NotThrow();
    }

    [Fact]
    public void StringTooLong_WithEmptyString_DoesNotThrow()
    {
        string emptyString = string.Empty;

        Action act = () => Guard.Against.StringTooLong(emptyString, 5);

        act.Should().NotThrow();
    }

    #endregion

    #region StringTooShort Tests

    [Fact]
    public void StringTooShort_WithNull_ThrowsGuardException()
    {
        string? nullString = null;

        Action act = () => Guard.Against.StringTooShort(nullString!, 3);

        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*");
    }

    [Fact]
    public void StringTooShort_WithStringBelowMin_ThrowsGuardException()
    {
        string shortString = "ab";

        Action act = () => Guard.Against.StringTooShort(shortString, 5);

        act.Should().Throw<GuardException>()
            .WithMessage("*must be at least*");
    }

    [Fact]
    public void StringTooShort_WithStringAtMin_DoesNotThrow()
    {
        string exactString = "12345";

        Action act = () => Guard.Against.StringTooShort(exactString, 5);

        act.Should().NotThrow();
    }

    [Fact]
    public void StringTooShort_WithStringAboveMin_DoesNotThrow()
    {
        string longString = "this is long enough";

        Action act = () => Guard.Against.StringTooShort(longString, 3);

        act.Should().NotThrow();
    }

    [Fact]
    public void StringTooShort_WithEmptyString_AndMinOne_ThrowsGuardException()
    {
        string emptyString = string.Empty;

        Action act = () => Guard.Against.StringTooShort(emptyString, 1);

        act.Should().Throw<GuardException>();
    }

    #endregion
}
