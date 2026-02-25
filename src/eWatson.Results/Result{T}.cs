using eWatson.Abstractions.Results;
using eWatson.Results.Resources;

namespace eWatson.Results;

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class Result<T> : Result, IResult<T>
{
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
    /// <param name="value">The value if the operation succeeded.</param>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="errorMessage">The error message if the operation failed.</param>
    /// <param name="errorDetails">Detailed error information.</param>
    internal Result(T value, bool isSuccess, string? errorMessage,
        ResultError? errorDetails = null)
        : base(isSuccess, errorMessage, errorDetails)
    {
        Value = value;
    }

    /// <summary>
    /// Implicitly converts a value to a success result.
    /// </summary>
    /// <param name="value">The value to wrap in a success result.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts a ResultError to a failure result.
    /// </summary>
    /// <param name="error">The error to wrap in a failure result.</param>
    public static implicit operator Result<T>(ResultError error) => Failure<T>(error);
}
