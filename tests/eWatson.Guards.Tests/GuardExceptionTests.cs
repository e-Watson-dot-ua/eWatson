using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardExceptionTests
{
    [Fact]
    public void Constructor_Default_CreatesException()
    {
        // Act
        var exception = new GuardException();

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<GuardException>();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        string expectedMessage = "Test error message";

        // Act
        var exception = new GuardException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        // Arrange
        string expectedMessage = "Outer exception";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new GuardException(expectedMessage, innerException);

        // Assert
        exception.Message.Should().Be(expectedMessage);
        exception.InnerException.Should().Be(innerException);
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void GuardException_InheritsFromException()
    {
        // Arrange
        var exception = new GuardException("Test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void GuardException_CanBeCaught()
    {
        // Arrange
        bool wasCaught = false;

        // Act
        try
        {
            throw new GuardException("Test exception");
        }
        catch (GuardException)
        {
            wasCaught = true;
        }

        // Assert
        wasCaught.Should().BeTrue();
    }

    [Fact]
    public void GuardException_CanBeCaughtAsBaseException()
    {
        // Arrange
        bool wasCaught = false;

        // Act
        try
        {
            throw new GuardException("Test exception");
        }
        catch (Exception ex)
        {
            wasCaught = true;
            ex.Should().BeOfType<GuardException>();
        }

        // Assert
        wasCaught.Should().BeTrue();
    }
}
