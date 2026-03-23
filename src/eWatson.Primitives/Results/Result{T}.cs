using eWatson.Abstractions.Results;
using eWatson.Primitives.Results.Resources;

namespace eWatson.Primitives.Results;

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public sealed class Result<T> : IResult<T>
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

    /// <inheritdoc />
    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException(
                    ResultMessages.CannotAccessValueOfFailedResult(ErrorMessage!));
            }

            return field;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    internal Result(T value, bool isSuccess, string? errorMessage,
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

        Value = value;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Errors = errors ?? [];
    }

    /// <summary>
    /// Implicitly converts a value to a success result.
    /// </summary>
    /// <param name="value">The value to wrap in a success result.</param>
    public static implicit operator Result<T>(T value) => Result.Success(value);

    /// <summary>
    /// Implicitly converts a ResultError to a failure result.
    /// </summary>
    /// <param name="error">The error to wrap in a failure result.</param>
    public static implicit operator Result<T>(ResultError error) => Result.Failure<T>(error);
}
