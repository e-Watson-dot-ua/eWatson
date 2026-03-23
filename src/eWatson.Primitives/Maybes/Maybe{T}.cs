namespace eWatson.Primitives.Maybes;

/// <summary>
/// Represents an optional value — either <c>Some(value)</c> or <c>None</c>.
/// Use instead of nullable references to make optionality explicit and composable.
/// Use <see cref="Maybe"/> factory methods to create instances.
/// </summary>
/// <typeparam name="T">The type of the contained value.</typeparam>
public readonly struct Maybe<T> : IEquatable<Maybe<T>> where T : notnull
{
    private readonly T? _value;
    private readonly bool _hasValue;

    /// <summary>
    /// Gets a value indicating whether this instance contains a value.
    /// </summary>
    public bool HasValue => _hasValue;

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    public bool HasNoValue => !_hasValue;

    /// <summary>
    /// Gets the contained value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when this instance has no value.</exception>
    public T Value => _hasValue
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of an empty Maybe.");

    private Maybe(T value)
    {
        _value = value;
        _hasValue = true;
    }

    /// <summary>
    /// Creates a new <see cref="Maybe{T}"/> containing the specified value.
    /// Intended for use by <see cref="Maybe"/> factory methods.
    /// </summary>
    internal static Maybe<T> Create(T value) => new(value);

    /// <summary>
    /// Implicitly converts a value to a <see cref="Maybe{T}"/> containing that value.
    /// </summary>
    public static implicit operator Maybe<T>(T value) => Maybe.Some(value);

    /// <inheritdoc />
    public bool Equals(Maybe<T> other)
    {
        if (!_hasValue && !other._hasValue) return true;
        if (!_hasValue || !other._hasValue) return false;
        return EqualityComparer<T>.Default.Equals(_value!, other._value!);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Maybe<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => _hasValue ? _value!.GetHashCode() : 0;

    /// <inheritdoc />
    public override string ToString() => _hasValue ? _value!.ToString() ?? "" : "None";

    /// <summary>Determines whether two <see cref="Maybe{T}"/> instances are equal.</summary>
    public static bool operator ==(Maybe<T> left, Maybe<T> right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="Maybe{T}"/> instances are not equal.</summary>
    public static bool operator !=(Maybe<T> left, Maybe<T> right) => !left.Equals(right);
}
