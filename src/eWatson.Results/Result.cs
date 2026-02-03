using eWatson.Abstractions.Results;
using eWatson.Results.Resources;

namespace eWatson.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// Provides factory methods for creating success and failure results.
/// </summary>
public class Result : IResult
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public string? Error { get; }

    /// <summary>
    /// Gets the detailed error information if the operation failed.
    /// </summary>
    public Results.Error? ErrorDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error message if the operation failed.</param>
    /// <param name="errorDetails">Detailed error information.</param>
    protected Result(bool isSuccess, string? error,
        Results.Error? errorDetails = null)
    {
        if (isSuccess && error is not null)
        {
            throw new ArgumentException(
                ResultMessages.SuccessResultCannotHaveError(), nameof(error));
        }

        if (!isSuccess && error is null)
        {
            throw new ArgumentException(
                ResultMessages.FailureResultMustHaveError(), nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        ErrorDetails = errorDetails;
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failure result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a failure result with detailed error information.
    /// </summary>
    /// <param name="error">The detailed error information.</param>
    public static Result Failure(Results.Error error) =>
        new(false, error.Message, error);

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The success value.</param>
    public static Result<T> Success<T>(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failure result with a value type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The error message.</param>
    public static Result<T> Failure<T>(string error) => new(default!, false, error);

    /// <summary>
    /// Creates a failure result with a value type and detailed error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The detailed error information.</param>
    public static Result<T> Failure<T>(Results.Error error) =>
        new(default!, false, error.Message, error);
}
