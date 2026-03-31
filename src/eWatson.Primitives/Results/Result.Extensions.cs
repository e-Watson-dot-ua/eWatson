using eWatson.Abstractions.Results;
using eWatson.Primitives.Maybes;

namespace eWatson.Primitives.Results;

/// <summary>
/// Extension methods for working with Result types in a functional style.
/// </summary>
public static class ResultExtensions
{
    /// <param name="result">The result to match.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    extension<T>(IResult<T> result)
    {
        /// <summary>
        /// Matches the result to one of two functions based on success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="onSuccess">Function to execute on success.</param>
        /// <param name="onFailure">Function to execute on failure.</param>
        public TResult Match<TResult>(Func<T, TResult> onSuccess,
            Func<string, TResult> onFailure)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.ErrorMessage!);
        }

        /// <summary>
        /// Matches the result to one of two actions based on success or failure.
        /// </summary>
        /// <param name="onSuccess">Action to execute on success.</param>
        /// <param name="onFailure">Action to execute on failure.</param>
        public void Match(Action<T> onSuccess, Action<string> onFailure)
        {
            if (result.IsSuccess)
                onSuccess(result.Value);
            else
                onFailure(result.ErrorMessage!);
        }

        /// <summary>
        /// Transforms the value of a successful result using the provided function.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="mapper">The mapping function.</param>
        public Result<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            return result.IsSuccess
                ? Result.Success(mapper(result.Value))
                : Result.Failure<TOut>(result.ErrorMessage!);
        }

        /// <summary>
        /// Transforms the value of a successful result using a function that
        /// returns another Result (flatMap/bind operation).
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="binder">Function that returns another result.</param>
        public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
        {
            return result.IsSuccess
                ? binder(result.Value)
                : Result.Failure<TOut>(result.ErrorMessage!);
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
            onSuccess();
        else
            onFailure(result.ErrorMessage!);
    }

    /// <param name="result">The result to tap.</param>
    /// <typeparam name="T">The type of the result value.</typeparam>
    extension<T>(Result<T> result)
    {
        /// <summary>
        /// Executes an action on a successful result and returns the same result.
        /// Useful for side effects like logging.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public Result<T> Tap(Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);

            return result;
        }

        /// <summary>
        /// Executes an action on a failed result and returns the same result.
        /// Useful for side effects like logging errors.
        /// </summary>
        /// <param name="action">The action to execute on failure.</param>
        public Result<T> TapError(Action<string> action)
        {
            if (result.IsFailure)
                action(result.ErrorMessage!);

            return result;
        }

        /// <summary>
        /// Ensures that a condition is met for a successful result.
        /// If the condition fails, returns a failure result.
        /// </summary>
        /// <param name="predicate">The condition to check.</param>
        /// <param name="errorMessage">The error message if the condition fails.</param>
        public Result<T> Ensure(Func<T, bool> predicate, string errorMessage)
        {
            if (result.IsFailure)
                return result;

            return predicate(result.Value)
                ? result
                : Result.Failure<T>(errorMessage);
        }

        /// <summary>
        /// Ensures that a condition is met for a successful result.
        /// If the condition fails, returns a failure result with detailed error.
        /// </summary>
        /// <param name="predicate">The condition to check.</param>
        /// <param name="error">The detailed error if the condition fails.</param>
        public Result<T> Ensure(Func<T, bool> predicate, ResultError error)
        {
            if (result.IsFailure)
                return result;

            return predicate(result.Value)
                ? result
                : Result.Failure<T>(error);
        }

    }

    /// <summary>
    /// Converts a successful result to a <see cref="Maybe{T}"/>.
    /// Returns <c>None</c> on failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    public static Maybe<T> ToMaybe<T>(this Result<T> result) where T : notnull
        => result.IsSuccess ? Maybe.Some(result.Value) : Maybe.None<T>();

    /// <param name="value">The nullable value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    extension<T>(T? value) where T : class
    {
        /// <summary>
        /// Converts a nullable value to a Result.
        /// </summary>
        /// <param name="errorMessage">The error message if the value is null.</param>
        public Result<T> ToResult(string errorMessage)
        {
            return value is not null
                ? Result.Success(value)
                : Result.Failure<T>(errorMessage);
        }

        /// <summary>
        /// Converts a nullable value to a Result with detailed error.
        /// </summary>
        /// <param name="error">The detailed error if the value is null.</param>
        public Result<T> ToResult(ResultError error)
        {
            return value is not null
                ? Result.Success(value)
                : Result.Failure<T>(error);
        }
    }

    /// <param name="resultTask">The result task to map.</param>
    /// <typeparam name="TIn">The input type.</typeparam>
    extension<TIn>(Task<Result<TIn>> resultTask)
    {
        /// <summary>
        /// Asynchronously transforms the value of a successful result.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="mapper">The mapping function.</param>
        public async Task<Result<TOut>> MapAsync<TOut>(Func<TIn, TOut> mapper)
        {
            var result = await resultTask;
            return result.IsSuccess
                ? Result.Success(mapper(result.Value))
                : Result.Failure<TOut>(result.ErrorMessage!);
        }

        /// <summary>
        /// Asynchronously binds the result to another result-returning function.
        /// </summary>
        /// <typeparam name="TOut">The output type.</typeparam>
        /// <param name="binder">Function that returns another result.</param>
        public async Task<Result<TOut>> BindAsync<TOut>(Func<TIn, Task<Result<TOut>>> binder)
        {
            var result = await resultTask;
            return result.IsSuccess
                ? await binder(result.Value)
                : Result.Failure<TOut>(result.ErrorMessage!);
        }
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
            return firstFailure.ErrorDetails is not null
                ? Result.Failure<IEnumerable<T>>(firstFailure.ErrorDetails)
                : Result.Failure<IEnumerable<T>>(firstFailure.ErrorMessage!);
        }

        return Result.Success(resultList.Select(r => r.Value));
    }
}
