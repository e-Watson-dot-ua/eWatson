using eWatson.Results;

namespace eWatson.Mediator.Internal;

/// <summary>
/// Shared helpers for constructing failure <see cref="Result"/> values inside pipeline
/// behaviours where the response type is only known at runtime.
/// </summary>
internal static class ResultHelper
{
    /// <summary>
    /// Creates a <see cref="Result"/> or <see cref="Result{T}"/> failure for
    /// <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TResponse">Must be <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
    /// <param name="error">The error to wrap.</param>
    /// <returns>A typed failure result.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <typeparamref name="TResponse"/> is neither <see cref="Result"/> nor
    /// <see cref="Result{T}"/>.
    /// </exception>
    internal static TResponse CreateFailure<TResponse>(ResultError error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, [typeof(ResultError)])!
                .MakeGenericMethod(valueType);

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new InvalidOperationException(
            $"Response type '{typeof(TResponse).Name}' is not Result or Result<T> and cannot " +
            "carry a failure. Ensure this behaviour is only applied to result-returning requests.");
    }
}
