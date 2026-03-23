using eWatson.Primitives.Results;

namespace eWatson.Primitives.Maybes;

/// <summary>
/// Extension methods for <see cref="Maybe{T}"/> providing functional composition.
/// </summary>
public static class MaybeExtensions
{
    /// <param name="maybe">The maybe to operate on.</param>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    extension<T>(Maybe<T> maybe) where T : notnull
    {
        /// <summary>
        /// Matches the maybe to one of two functions based on whether it has a value.
        /// </summary>
        /// <typeparam name="TResult">The return type.</typeparam>
        /// <param name="onSome">Function to execute when a value is present.</param>
        /// <param name="onNone">Function to execute when no value is present.</param>
        public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            => maybe.HasValue ? onSome(maybe.Value) : onNone();

        /// <summary>
        /// Matches the maybe to one of two actions based on whether it has a value.
        /// </summary>
        /// <param name="onSome">Action to execute when a value is present.</param>
        /// <param name="onNone">Action to execute when no value is present.</param>
        public void Match(Action<T> onSome, Action onNone)
        {
            if (maybe.HasValue) onSome(maybe.Value);
            else onNone();
        }

        /// <summary>
        /// Transforms the contained value using the provided function.
        /// Returns <c>None</c> if this instance is empty.
        /// </summary>
        /// <typeparam name="TResult">The output type.</typeparam>
        /// <param name="mapper">The mapping function.</param>
        public Maybe<TResult> Map<TResult>(Func<T, TResult> mapper) where TResult : notnull
            => maybe.HasValue ? Maybe.Some(mapper(maybe.Value)) : Maybe.None<TResult>();

        /// <summary>
        /// Transforms the contained value using a function that returns another Maybe
        /// (flatMap/bind operation).
        /// </summary>
        /// <typeparam name="TResult">The output type.</typeparam>
        /// <param name="binder">Function that returns another Maybe.</param>
        public Maybe<TResult> Bind<TResult>(Func<T, Maybe<TResult>> binder) where TResult : notnull
            => maybe.HasValue ? binder(maybe.Value) : Maybe.None<TResult>();

        /// <summary>
        /// Returns the contained value if present; otherwise returns the fallback value.
        /// </summary>
        /// <param name="fallback">The fallback value.</param>
        public T OrElse(T fallback)
            => maybe.HasValue ? maybe.Value : fallback;

        /// <summary>
        /// Returns the contained value if present; otherwise invokes the factory and returns its result.
        /// </summary>
        /// <param name="fallbackFactory">Factory that produces the fallback value.</param>
        public T OrElse(Func<T> fallbackFactory)
            => maybe.HasValue ? maybe.Value : fallbackFactory();

        /// <summary>
        /// Returns the contained value or <c>default</c> if empty.
        /// </summary>
        public T? OrDefault()
            => maybe.HasValue ? maybe.Value : default;

        /// <summary>
        /// Filters the contained value using the predicate. Returns <c>None</c>
        /// if the predicate returns <c>false</c>.
        /// </summary>
        /// <param name="predicate">The condition to check.</param>
        public Maybe<T> Where(Func<T, bool> predicate)
            => maybe.HasValue && predicate(maybe.Value) ? maybe : Maybe.None<T>();

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> to a <see cref="Result{T}"/>.
        /// Returns a success result if a value is present; otherwise a failure.
        /// </summary>
        /// <param name="errorMessage">The error message if empty.</param>
        public Result<T> ToResult(string errorMessage)
            => maybe.HasValue ? Result.Success(maybe.Value) : Result.Failure<T>(errorMessage);

        /// <summary>
        /// Converts this <see cref="Maybe{T}"/> to a <see cref="Result{T}"/>.
        /// Returns a success result if a value is present; otherwise a failure with detailed error.
        /// </summary>
        /// <param name="error">The detailed error if empty.</param>
        public Result<T> ToResult(ResultError error)
            => maybe.HasValue ? Result.Success(maybe.Value) : Result.Failure<T>(error);

        /// <summary>
        /// Executes an action on the contained value if present and returns the same maybe.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public Maybe<T> Tap(Action<T> action)
        {
            if (maybe.HasValue) action(maybe.Value);
            return maybe;
        }
    }

    /// <summary>
    /// Converts a nullable reference to a <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The nullable value.</param>
    public static Maybe<T> ToMaybe<T>(this T? value) where T : notnull
        => value is not null ? Maybe.Some(value) : Maybe.None<T>();

    /// <summary>
    /// Converts a nullable value type to a <see cref="Maybe{T}"/>.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="value">The nullable value.</param>
    public static Maybe<T> ToMaybe<T>(this T? value) where T : struct
        => value.HasValue ? Maybe.Some(value.Value) : Maybe.None<T>();
}
