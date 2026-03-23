using eWatson.Abstractions.Events;
using eWatson.Entities;
using FluentAssertions;

namespace eWatson.Entities.Tests;

public sealed class EntityTests
{
    private sealed class TestEntity : Entity<int>
    {
        public TestEntity() { }
        public TestEntity(int id) : base(id) { }
    }

    private sealed class OtherEntity : Entity<int>
    {
        public OtherEntity(int id) : base(id) { }
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        var a = new TestEntity(1);
        var b = new TestEntity(1);

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var a = new TestEntity(1);
        var b = new TestEntity(2);

        a.Equals(b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var a = new TestEntity(1);
        var b = new OtherEntity(1);

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_Transient_ReturnsFalse()
    {
        var a = new TestEntity();
        var b = new TestEntity();

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var a = new TestEntity(1);

        a.Equals(a).Should().BeTrue();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var a = new TestEntity(1);

        a.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Operator_BothNull_ReturnsTrue()
    {
        TestEntity? a = null;
        TestEntity? b = null;

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Operator_OneNull_ReturnsFalse()
    {
        var a = new TestEntity(1);
        TestEntity? b = null;

        (a == b).Should().BeFalse();
        (b == a).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameId_SameHash()
    {
        var a = new TestEntity(5);
        var b = new TestEntity(5);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_Transient_UsesBaseHashCode()
    {
        var a = new TestEntity();
        var b = new TestEntity();

        // Transient entities should have different hash codes (from object ref)
        // This is probabilistic but practically always true
        a.GetHashCode().Should().NotBe(b.GetHashCode());
    }
}

public sealed class AggregateRootTests
{
    private sealed record TestEvent(string Data) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }

    private sealed class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id) { }

        public void DoSomething(string data)
        {
            RaiseDomainEvent(new TestEvent(data));
        }
    }

    [Fact]
    public void RaiseDomainEvent_AddsEvent()
    {
        var agg = new TestAggregate(Guid.NewGuid());

        agg.DoSomething("test");

        agg.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAll()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        agg.DoSomething("a");
        agg.DoSomething("b");

        agg.ClearDomainEvents();

        agg.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void RemoveDomainEvent_RemovesSpecificEvent()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        var evt = new TestEvent("x");
        agg.AddDomainEvent(evt);
        agg.DoSomething("y");

        agg.RemoveDomainEvent(evt);

        agg.DomainEvents.Should().HaveCount(1);
    }
}
