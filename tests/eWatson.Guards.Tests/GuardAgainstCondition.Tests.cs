using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstConditionTests
{
    #region False Tests

    [Fact]
    public void False_WhenConditionIsFalse_ThrowsGuardException()
    {
        // Arrange
        bool condition = false;
        string message = "Condition must be true";

        // Act
        Action act = () => Guard.Against.False(condition, message);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage(message);
    }

    [Fact]
    public void False_WhenConditionIsTrue_DoesNotThrow()
    {
        // Arrange
        bool condition = true;
        string message = "Condition must be true";

        // Act
        Action act = () => Guard.Against.False(condition, message);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void False_WithComplexCondition_WorksCorrectly()
    {
        // Arrange
        int value = 10;
        bool condition = value > 5 && value < 15;
        string message = "Value must be between 5 and 15";

        // Act
        Action act = () => Guard.Against.False(condition, message);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void False_WithFailingComplexCondition_ThrowsWithCorrectMessage()
    {
        // Arrange
        int value = 20;
        bool condition = value > 5 && value < 15;
        string message = "Value must be between 5 and 15";

        // Act
        Action act = () => Guard.Against.False(condition, message);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage(message);
    }

    #endregion

    #region True Tests

    [Fact]
    public void True_WhenConditionIsTrue_ThrowsGuardException()
    {
        // Arrange
        bool condition = true;
        string message = "Condition must be false";

        // Act
        Action act = () => Guard.Against.True(condition, message);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage(message);
    }

    [Fact]
    public void True_WhenConditionIsFalse_DoesNotThrow()
    {
        // Arrange
        bool condition = false;
        string message = "Condition must be false";

        // Act
        Action act = () => Guard.Against.True(condition, message);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void True_WithComplexCondition_WorksCorrectly()
    {
        // Arrange
        bool isDuplicate = false;
        string message = "Email already exists";

        // Act
        Action act = () => Guard.Against.True(isDuplicate, message);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void True_WithFailingComplexCondition_ThrowsWithCorrectMessage()
    {
        // Arrange
        bool isDuplicate = true;
        string message = "Email already exists";

        // Act
        Action act = () => Guard.Against.True(isDuplicate, message);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage(message);
    }

    [Fact]
    public void True_WithNullCheckCondition_WorksCorrectly()
    {
        // Arrange
        object? value = null;
        bool isNull = value is null;
        string message = "Value should not be null";

        // Act
        Action act = () => Guard.Against.True(isNull, message);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage(message);
    }

    #endregion
}
