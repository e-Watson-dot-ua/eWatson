using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstNullTests
{
    [Fact]
    public void Null_WithNullArgument_ThrowsGuardException()
    {
        // Arrange
        object? nullObject = null;

        // Act
        Action act = () => Guard.Against.Null(nullObject);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*");
    }

    [Fact]
    public void Null_WithNonNullArgument_DoesNotThrow()
    {
        // Arrange
        var validObject = new object();

        // Act
        Action act = () => Guard.Against.Null(validObject);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Null_WithNullString_ThrowsGuardException()
    {
        // Arrange
        string? nullString = null;

        // Act
        Action act = () => Guard.Against.Null(nullString);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null*");
    }

    [Fact]
    public void Null_WithValidString_DoesNotThrow()
    {
        // Arrange
        string validString = "test";

        // Act
        Action act = () => Guard.Against.Null(validString);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Null_WithEmptyString_DoesNotThrow()
    {
        // Arrange
        string emptyString = string.Empty;

        // Act
        Action act = () => Guard.Against.Null(emptyString);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Null_WithNullableValueType_ThrowsWhenNull()
    {
        // Arrange
        int? nullInt = null;

        // Act
        Action act = () => Guard.Against.Null(nullInt);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void Null_WithNullableValueType_DoesNotThrowWhenHasValue()
    {
        // Arrange
        int? validInt = 42;

        // Act
        Action act = () => Guard.Against.Null(validInt);

        // Assert
        act.Should().NotThrow();
    }
}
