using eWatson.ValueObjects;
using FluentAssertions;

namespace eWatson.ValueObjects.Tests;

public sealed class ValueObjectEqualityTests
{
    private sealed class TestValueObject(string a, int b) : ValueObject
    {
        public string A { get; } = a;
        public int B { get; } = b;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return A;
            yield return B;
        }
    }

    [Fact]
    public void SameComponents_AreEqual()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("x", 1);

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void DifferentComponents_AreNotEqual()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("y", 1);

        a.Should().NotBe(b);
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Null_IsNotEqual()
    {
        var a = new TestValueObject("x", 1);

        a.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameComponents_SameHash()
    {
        var a = new TestValueObject("x", 1);
        var b = new TestValueObject("x", 1);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
