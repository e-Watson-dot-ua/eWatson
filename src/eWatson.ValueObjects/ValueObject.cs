using eWatson.Abstractions.ValueObjects;

namespace eWatson.ValueObjects;

/// <summary>
/// Base class for value objects that provides structural equality comparison.
/// Value objects are immutable and compared by their property values rather
/// than reference identity.
/// </summary>
/// <remarks>
/// Derived classes should:
/// <list type="bullet">
/// <item>Override <see cref="GetEqualityComponents"/> to specify which
/// properties define equality</item>
/// <item>Be immutable (use init-only or readonly properties)</item>
/// <item>Validate invariants in their constructors</item>
/// </list>
/// </remarks>
public abstract class ValueObject : IValueObject, IEquatable<ValueObject>
{
    /// <summary>
    /// Returns the components that define equality for this value object.
    /// </summary>
    /// <returns>
    /// An enumerable of objects representing the properties that define equality.
    /// </returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether this value object is equal to another value object.
    /// </summary>
    /// <param name="other">The value object to compare with.</param>
    /// <returns>
    /// <c>true</c> if the objects are equal; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(ValueObject? other)
    {
        if (other is null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Determines whether this value object is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>
    /// <c>true</c> if the objects are equal; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this value object based on its equality
    /// components.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(
                0,
                (hashCode, component) => HashCode.Combine(
                    hashCode,
                    component?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns>
    /// <c>true</c> if the value objects are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns>
    /// <c>true</c> if the value objects are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
