using eWatson.Guards.Exceptions;
using FluentAssertions;

namespace eWatson.Guards.Tests;

public class GuardAgainstCollectionTests
{
    [Fact]
    public void NullOrEmpty_WithNullCollection_ThrowsGuardException()
    {
        // Arrange
        IEnumerable<int>? nullCollection = null;

        // Act
        Action act = () => Guard.Against.NullOrEmpty(nullCollection);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void NullOrEmpty_WithEmptyCollection_ThrowsGuardException()
    {
        // Arrange
        var emptyCollection = Enumerable.Empty<int>();

        // Act
        Action act = () => Guard.Against.NullOrEmpty(emptyCollection);

        // Assert
        act.Should().Throw<GuardException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Fact]
    public void NullOrEmpty_WithEmptyList_ThrowsGuardException()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        Action act = () => Guard.Against.NullOrEmpty(emptyList);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullOrEmpty_WithEmptyArray_ThrowsGuardException()
    {
        // Arrange
        var emptyArray = Array.Empty<int>();

        // Act
        Action act = () => Guard.Against.NullOrEmpty(emptyArray);

        // Assert
        act.Should().Throw<GuardException>();
    }

    [Fact]
    public void NullOrEmpty_WithSingleItemCollection_DoesNotThrow()
    {
        // Arrange
        var collection = new[] { 1 };

        // Act
        Action act = () => Guard.Against.NullOrEmpty(collection);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NullOrEmpty_WithMultipleItemsCollection_DoesNotThrow()
    {
        // Arrange
        var collection = new List<string> { "item1", "item2", "item3" };

        // Act
        Action act = () => Guard.Against.NullOrEmpty(collection);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NullOrEmpty_WithLazyEnumerable_DoesNotThrow()
    {
        // Arrange
        var collection = Enumerable.Range(1, 10);

        // Act
        Action act = () => Guard.Against.NullOrEmpty(collection);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NullOrEmpty_WithComplexObjects_DoesNotThrow()
    {
        // Arrange
        var collection = new[] { new { Id = 1 }, new { Id = 2 } };

        // Act
        Action act = () => Guard.Against.NullOrEmpty(collection);

        // Assert
        act.Should().NotThrow();
    }
}
