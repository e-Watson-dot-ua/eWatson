namespace eWatson.Abstractions.Results;

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value if the operation succeeded.
    /// </summary>
    /// <remarks>
    /// Accessing this property when <see cref="IResult.IsFailure"/> is <c>true</c>
    /// will throw an <see cref="InvalidOperationException"/> in the default implementation.
    /// Always check <see cref="IResult.IsSuccess"/> before accessing.
    /// </remarks>
    T Value { get; }
}
