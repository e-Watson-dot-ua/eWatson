namespace eWatson.Abstractions.Results;

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public interface IResult<out T> : IResult
{
    /// <summary>
    /// Gets the value if the operation succeeded.
    /// Throws if accessed when IsFailure is true.
    /// </summary>
    T Value { get; }
}
