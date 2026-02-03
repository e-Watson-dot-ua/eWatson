namespace eWatson.Abstractions.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// Provides a way to handle success/failure without exceptions.
/// </summary>
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    string? Error { get; }
}
