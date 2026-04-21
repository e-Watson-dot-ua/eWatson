using eWatson.Abstractions.Results;
using eWatson.Primitives.Results.Resources;

namespace eWatson.Primitives.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// Provides factory methods for creating success and failure results.
/// </summary>
public sealed class Result : IResult
{
    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <inheritdoc />
    public bool IsFailure => !IsSuccess;

    /// <inheritdoc />
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the collection of structured errors associated with this result.
    /// Empty for success results.
    /// </summary>
    public IReadOnlyList<ResultError> Errors { get; }

    /// <summary>
    /// Gets the first error if any exist, otherwise <c>null</c>.
    /// Convenience accessor for single-error scenarios.
    /// </summary>
    public ResultError? ErrorDetails => Errors.Count > 0 ? Errors[0] : null;

    private Result(bool isSuccess, string? errorMessage,
        IReadOnlyList<ResultError>? errors = null)
    {
        if (isSuccess && errorMessage is not null)
        {
            throw new ArgumentException(
                ResultMessages.SuccessResultCannotHaveError(), nameof(errorMessage));
        }

        if (!isSuccess && errorMessage is null)
        {
            throw new ArgumentException(
                ResultMessages.FailureResultMustHaveError(), nameof(errorMessage));
        }

        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Errors = errors ?? [];
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failure result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public static Result Failure(string errorMessage) =>
        new(false, errorMessage, [ResultError.General(errorMessage)]);

    /// <summary>
    /// Creates a failure result with detailed error information.
    /// </summary>
    /// <param name="error">The detailed error information.</param>
    public static Result Failure(ResultError error) =>
        new(false, error.Message, [error]);

    /// <summary>
    /// Creates a failure result with multiple errors.
    /// </summary>
    /// <param name="errors">The collection of errors.</param>
    public static Result Failure(IReadOnlyList<ResultError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
            throw new ArgumentException(
                "Failure results must contain at least one error.",
                nameof(errors));

        return new(false,
            string.Join("; ", errors.Select(e => e.Message)),
            errors);
    }

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
    /// <param name="errorMessage">The error message.</param>
    public static Result<T> Failure<T>(string errorMessage) =>
        new(default!, false, errorMessage, [ResultError.General(errorMessage)]);

    /// <summary>
    /// Creates a failure result with a value type and detailed error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The detailed error information.</param>
    public static Result<T> Failure<T>(ResultError error) =>
        new(default!, false, error.Message, [error]);

    /// <summary>
    /// Creates a failure result with a value type and multiple errors.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="errors">The collection of errors.</param>
    public static Result<T> Failure<T>(IReadOnlyList<ResultError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        if (errors.Count == 0)
            throw new ArgumentException(
                "Failure results must contain at least one error.",
                nameof(errors));

        return new(default!, false,
            string.Join("; ", errors.Select(e => e.Message)),
            errors);
    }
}
