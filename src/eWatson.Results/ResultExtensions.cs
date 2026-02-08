using eWatson.Abstractions.Results;

namespace eWatson.Results;

/// <summary>
/// Extension methods for working with Result types in a functional style.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Matches the result to one of two functions based on success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">Function to execute on success.</param>
    /// <param name="onFailure">Function to execute on failure.</param>
    public static TResult Match<T, TResult>(
        this IResult<T> result,
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error!);
    }

    /// <summary>
    /// Matches the result to one of two actions based on success or failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    public static void Match<T>(
        this IResult<T> result,
        Action<T> onSuccess,
        Action<string> onFailure)
    {
        if (result.IsSuccess)
        {
            onSuccess(result.Value);
        }
        else
        {
            onFailure(result.Error!);
        }
    }

    /// <summary>
    /// Matches the result to one of two actions based on success or failure.
    /// </summary>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">Action to execute on success.</param>
    /// <param name="onFailure">Action to execute on failure.</param>
    public static void Match(
        this IResult result,
        Action onSuccess,
        Action<string> onFailure)
    {
        if (result.IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure(result.Error!);
        }
    }

    /// <summary>
    /// Transforms the value of a successful result using the provided function.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <param name="mapper">The mapping function.</param>
    public static Result<TOut> Map<TIn, TOut>(
        this IResult<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value))
            : Result.Failure<TOut>(result.Error!);
    }

    /// <summary>
    /// Transforms the value of a successful result using a function that
    /// returns another Result (flatMap/bind operation).
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binder">Function that returns another result.</param>
    public static Result<TOut> Bind<TIn, TOut>(
        this IResult<TIn> result,
        Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result.Failure<TOut>(result.Error!);
    }

    /// <summary>
    /// Executes an action on a successful result and returns the same result.
    /// Useful for side effects like logging.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to tap.</param>
    /// <param name="action">The action to execute.</param>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    /// <summary>
    /// Executes an action on a failed result and returns the same result.
    /// Useful for side effects like logging errors.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to tap.</param>
    /// <param name="action">The action to execute on failure.</param>
    public static Result<T> TapError<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure)
        {
            action(result.Error!);
        }

        return result;
    }

    /// <summary>
    /// Ensures that a condition is met for a successful result.
    /// If the condition fails, returns a failure result.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <param name="error">The error message if the condition fails.</param>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate,
        string error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value)
            ? result
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Ensures that a condition is met for a successful result.
    /// If the condition fails, returns a failure result with detailed error.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <param name="error">The detailed error if the condition fails.</param>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value)
            ? result
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Converts a nullable value to a Result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="error">The error message if the value is null.</param>
    public static Result<T> ToResult<T>(this T? value, string error)
        where T : class
    {
        return value is not null
            ? Result.Success(value)
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Converts a nullable value to a Result with detailed error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="error">The detailed error if the value is null.</param>
    public static Result<T> ToResult<T>(this T? value, Error error)
        where T : class
    {
        return value is not null
            ? Result.Success(value)
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Asynchronously transforms the value of a successful result.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="resultTask">The result task to map.</param>
    /// <param name="mapper">The mapping function.</param>
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, TOut> mapper)
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

    /// <summary>
    /// Asynchronously binds the result to another result-returning function.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="resultTask">The result task to bind.</param>
    /// <param name="binder">Function that returns another result.</param>
    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await binder(result.Value)
            : Result.Failure<TOut>(result.Error!);
    }

    /// <summary>
    /// Combines multiple results into a single result containing all values.
    /// If any result fails, returns the first failure.
    /// </summary>
    /// <typeparam name="T">The type of the result values.</typeparam>
    /// <param name="results">The results to combine.</param>
    public static Result<IEnumerable<T>> Combine<T>(
        this IEnumerable<Result<T>> results)
    {
        var resultList = results.ToList();
        var firstFailure = resultList.FirstOrDefault(r => r.IsFailure);

        if (firstFailure is not null)
        {
            return Result.Failure<IEnumerable<T>>(firstFailure.Error!);
        }

        return Result.Success(resultList.Select(r => r.Value));
    }
}
