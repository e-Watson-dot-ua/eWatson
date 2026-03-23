namespace eWatson.Primitives.Maybes;

/// <summary>
/// Provides factory methods for creating <see cref="Maybe{T}"/> instances.
/// </summary>
public static class Maybe
{
    /// <summary>
    /// Creates a <see cref="Maybe{T}"/> containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static Maybe<T> Some<T>(T value) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return Maybe<T>.Create(value);
    }

    /// <summary>
    /// Gets an empty <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public static Maybe<T> None<T>() where T : notnull => default;
}
