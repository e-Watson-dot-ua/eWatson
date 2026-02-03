using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstNumericTests
{
    #region Negative Tests - Int

    [Fact]
    public void Negative_Int_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        int negativeValue = -1;

        // Act
        Action act = () => Guard.Against.Negative(negativeValue);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Negative_Int_WithZero_DoesNotThrow()
    {
        // Arrange
        int zero = 0;

        // Act
        Action act = () => Guard.Against.Negative(zero);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Negative_Int_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        int positiveValue = 42;

        // Act
        Action act = () => Guard.Against.Negative(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Negative Tests - Long

    [Fact]
    public void Negative_Long_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        long negativeValue = -1000L;

        // Act
        Action act = () => Guard.Against.Negative(negativeValue);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Negative_Long_WithZero_DoesNotThrow()
    {
        // Arrange
        long zero = 0L;

        // Act
        Action act = () => Guard.Against.Negative(zero);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Negative_Long_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        long positiveValue = 9999999999L;

        // Act
        Action act = () => Guard.Against.Negative(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region Negative Tests - Decimal

    [Fact]
    public void Negative_Decimal_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        decimal negativeValue = -0.01m;

        // Act
        Action act = () => Guard.Against.Negative(negativeValue);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Negative_Decimal_WithZero_DoesNotThrow()
    {
        // Arrange
        decimal zero = 0m;

        // Act
        Action act = () => Guard.Against.Negative(zero);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Negative_Decimal_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        decimal positiveValue = 99.99m;

        // Act
        Action act = () => Guard.Against.Negative(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region NegativeOrZero Tests - Int

    [Fact]
    public void NegativeOrZero_Int_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        int negativeValue = -5;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(negativeValue);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be greater than zero*");
    }

    [Fact]
    public void NegativeOrZero_Int_WithZero_ThrowsGuardException()
    {
        // Arrange
        int zero = 0;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(zero);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be greater than zero*");
    }

    [Fact]
    public void NegativeOrZero_Int_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        int positiveValue = 1;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region NegativeOrZero Tests - Long

    [Fact]
    public void NegativeOrZero_Long_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        long negativeValue = -100L;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(negativeValue);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NegativeOrZero_Long_WithZero_ThrowsGuardException()
    {
        // Arrange
        long zero = 0L;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(zero);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NegativeOrZero_Long_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        long positiveValue = 1L;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region NegativeOrZero Tests - Decimal

    [Fact]
    public void NegativeOrZero_Decimal_WithNegativeValue_ThrowsGuardException()
    {
        // Arrange
        decimal negativeValue = -0.01m;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(negativeValue);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NegativeOrZero_Decimal_WithZero_ThrowsGuardException()
    {
        // Arrange
        decimal zero = 0m;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(zero);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NegativeOrZero_Decimal_WithPositiveValue_DoesNotThrow()
    {
        // Arrange
        decimal positiveValue = 0.01m;

        // Act
        Action act = () => Guard.Against.NegativeOrZero(positiveValue);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region OutOfRange Tests - Int

    [Fact]
    public void OutOfRange_Int_WithValueBelowMin_ThrowsGuardException()
    {
        // Arrange
        int value = 5;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be between*");
    }

    [Fact]
    public void OutOfRange_Int_WithValueAboveMax_ThrowsGuardException()
    {
        // Arrange
        int value = 25;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*must be between*");
    }

    [Fact]
    public void OutOfRange_Int_WithValueAtMin_DoesNotThrow()
    {
        // Arrange
        int value = 10;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OutOfRange_Int_WithValueAtMax_DoesNotThrow()
    {
        // Arrange
        int value = 20;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void OutOfRange_Int_WithValueInRange_DoesNotThrow()
    {
        // Arrange
        int value = 15;
        int min = 10;
        int max = 20;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region OutOfRange Tests - Long

    [Fact]
    public void OutOfRange_Long_WithValueOutOfRange_ThrowsGuardException()
    {
        // Arrange
        long value = 1000L;
        long min = 1L;
        long max = 100L;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void OutOfRange_Long_WithValueInRange_DoesNotThrow()
    {
        // Arrange
        long value = 50L;
        long min = 1L;
        long max = 100L;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region OutOfRange Tests - Decimal

    [Fact]
    public void OutOfRange_Decimal_WithValueOutOfRange_ThrowsGuardException()
    {
        // Arrange
        decimal value = 10.5m;
        decimal min = 0m;
        decimal max = 10m;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void OutOfRange_Decimal_WithValueInRange_DoesNotThrow()
    {
        // Arrange
        decimal value = 5.5m;
        decimal min = 0m;
        decimal max = 10m;

        // Act
        Action act = () => Guard.Against.OutOfRange(value, min, max);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region InvalidValue Tests

    [Fact]
    public void InvalidValue_WithMatchingValue_ThrowsGuardException()
    {
        // Arrange
        int value = 42;
        int invalidValue = 42;

        // Act
        Action act = () => Guard.Against.InvalidValue(value, invalidValue);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be*");
    }

    [Fact]
    public void InvalidValue_WithDifferentValue_DoesNotThrow()
    {
        // Arrange
        int value = 42;
        int invalidValue = 99;

        // Act
        Action act = () => Guard.Against.InvalidValue(value, invalidValue);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void InvalidValue_WithStringValues_WorksCorrectly()
    {
        // Arrange
        string value = "test";
        string invalidValue = "test";

        // Act
        Action act = () => Guard.Against.InvalidValue(value, invalidValue);

        // Assert
        act.Should().Throw<GuardException>();
    }

    #endregion

    #region EmptyGuid Tests

    [Fact]
    public void EmptyGuid_WithEmptyGuid_ThrowsGuardException()
    {
        // Arrange
        Guid emptyGuid = Guid.Empty;

        // Act
        Action act = () => Guard.Against.EmptyGuid(emptyGuid);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be*empty*guid*");
    }

    [Fact]
    public void EmptyGuid_WithValidGuid_DoesNotThrow()
    {
        // Arrange
        Guid validGuid = Guid.NewGuid();

        // Act
        Action act = () => Guard.Against.EmptyGuid(validGuid);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EmptyGuid_WithSpecificGuid_DoesNotThrow()
    {
        // Arrange
        Guid specificGuid = new("12345678-1234-1234-1234-123456789012");

        // Act
        Action act = () => Guard.Against.EmptyGuid(specificGuid);

        // Assert
        act.Should().NotThrow();
    }

    #endregion
}
