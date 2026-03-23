using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstStringTests
{
    #region NullOrEmpty Tests

    [Fact]
    public void NullOrEmpty_WithNullString_ThrowsGuardException()
    {
        // Arrange
        string? nullString = null;

        // Act
        Action act = () => Guard.Against.NullOrEmpty(nullString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void NullOrEmpty_WithEmptyString_ThrowsGuardException()
    {
        // Arrange
        string emptyString = string.Empty;

        // Act
        Action act = () => Guard.Against.NullOrEmpty(emptyString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void NullOrEmpty_WithWhitespaceString_DoesNotThrow()
    {
        // Arrange
        string whitespaceString = "   ";

        // Act
        Action act = () => Guard.Against.NullOrEmpty(whitespaceString);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NullOrEmpty_WithValidString_DoesNotThrow()
    {
        // Arrange
        string validString = "test";

        // Act
        Action act = () => Guard.Against.NullOrEmpty(validString);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region NullOrWhiteSpace Tests

    [Fact]
    public void NullOrWhiteSpace_WithNullString_ThrowsGuardException()
    {
        // Arrange
        string? nullString = null;

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(nullString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*whitespace*");
    }

    [Fact]
    public void NullOrWhiteSpace_WithEmptyString_ThrowsGuardException()
    {
        // Arrange
        string emptyString = string.Empty;

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(emptyString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*whitespace*");
    }

    [Fact]
    public void NullOrWhiteSpace_WithWhitespaceString_ThrowsGuardException()
    {
        // Arrange
        string whitespaceString = "   ";

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(whitespaceString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*whitespace*");
    }

    [Fact]
    public void NullOrWhiteSpace_WithTabString_ThrowsGuardException()
    {
        // Arrange
        string tabString = "\t";

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(tabString);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullOrWhiteSpace_WithNewlineString_ThrowsGuardException()
    {
        // Arrange
        string newlineString = "\n";

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(newlineString);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullOrWhiteSpace_WithValidString_DoesNotThrow()
    {
        // Arrange
        string validString = "test";

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(validString);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NullOrWhiteSpace_WithStringContainingWhitespace_DoesNotThrow()
    {
        // Arrange
        string stringWithWhitespace = "test value";

        // Act
        Action act = () => Guard.Against.NullOrWhiteSpace(stringWithWhitespace);

        // Assert
        act.Should().NotThrow();
    }

    #endregion
}
