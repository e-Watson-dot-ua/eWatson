using System.Linq.Expressions;
using eWatson.Abstractions.Specifications;
using FluentAssertions;

namespace eWatson.Abstractions.Tests.Specifications;

public class SpecificationTests
{
    private sealed class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private sealed class TestSpecification : Specification<TestEntity>
    {
        public TestSpecification(Expression<Func<TestEntity, bool>>? criteria = null)
        {
            if (criteria != null)
            {
                Where(criteria);
            }
        }
    }

    [Fact]
    public void ImplicitOperator_WithCriteria_ReturnsExpression()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> expectedCriteria = x => x.IsActive;
        var spec = new TestSpecification(expectedCriteria);

        // Act
        Expression<Func<TestEntity, bool>>? result = spec;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(expectedCriteria);
    }

    [Fact]
    public void ImplicitOperator_WithoutCriteria_ReturnsNull()
    {
        // Arrange
        var spec = new TestSpecification();

        // Act
        Expression<Func<TestEntity, bool>>? result = spec;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ImplicitOperator_WithNullSpecification_ReturnsNull()
    {
        // Arrange
        TestSpecification? spec = null;

        // Act
        Expression<Func<TestEntity, bool>>? result = spec;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ImplicitOperator_UsedInLinqWhere_FiltersCorrectly()
    {
        // Arrange
        var entities = new List<TestEntity>
        {
            new() { Id = 1, Name = "Active", IsActive = true },
            new() { Id = 2, Name = "Inactive", IsActive = false },
            new() { Id = 3, Name = "Active2", IsActive = true }
        };

        var spec = new TestSpecification(x => x.IsActive);

        // Act
        Expression<Func<TestEntity, bool>> expression = spec!;
        var result = entities.AsQueryable().Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(e => e.IsActive.Should().BeTrue());
    }

    [Fact]
    public void ImplicitOperator_WithComplexCriteria_PreservesLogic()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> complexCriteria =
            x => x.IsActive && x.Name.StartsWith("Test");
        var spec = new TestSpecification(complexCriteria);

        // Act
        Expression<Func<TestEntity, bool>>? result = spec;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(complexCriteria);

        // Verify the expression works
        var compiledExpression = result!.Compile();
        compiledExpression(new TestEntity
        {
            Id = 1,
            Name = "TestEntity",
            IsActive = true
        }).Should().BeTrue();

        compiledExpression(new TestEntity
        {
            Id = 2,
            Name = "OtherEntity",
            IsActive = true
        }).Should().BeFalse();
    }

    [Fact]
    public void ImplicitOperator_AssignedToVariable_WorksCorrectly()
    {
        // Arrange
        var spec = new TestSpecification(x => x.Id > 10);

        // Act
        Expression<Func<TestEntity, bool>>? expression = spec;
        var compiled = expression?.Compile();

        // Assert
        compiled.Should().NotBeNull();
        compiled!(new TestEntity { Id = 15 }).Should().BeTrue();
        compiled(new TestEntity { Id = 5 }).Should().BeFalse();
    }

    [Fact]
    public void ImplicitOperator_CriteriaProperty_ReturnsSameExpression()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> criteria = x => x.IsActive;
        var spec = new TestSpecification(criteria);

        // Act
        Expression<Func<TestEntity, bool>>? implicitResult = spec;
        var propertyResult = spec.Criteria;

        // Assert
        implicitResult.Should().BeSameAs(propertyResult);
        implicitResult.Should().BeSameAs(criteria);
    }
}
