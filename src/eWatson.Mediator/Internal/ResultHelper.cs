using eWatson.Mediator.Resources;
using eWatson.Primitives.Results;

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
    internal static TResponse CreateFailure<TResponse>(ResultError error)
        => CreateFailure<TResponse>((IReadOnlyList<ResultError>)[error]);

    /// <summary>
    /// Creates a <see cref="Result"/> or <see cref="Result{T}"/> failure for
    /// <typeparamref name="TResponse"/> with multiple errors.
    /// </summary>
    internal static TResponse CreateFailure<TResponse>(IReadOnlyList<ResultError> errors)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(errors);

        if (!typeof(TResponse).IsGenericType || typeof(TResponse).GetGenericTypeDefinition() != typeof(Result<>))
            throw new InvalidOperationException(
                MediatorMessages.UnsupportedResponseType(typeof(TResponse).Name));

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result)
            .GetMethod(nameof(Result.Failure), 1, [typeof(IReadOnlyList<ResultError>)])!
            .MakeGenericMethod(valueType);

        return (TResponse)failureMethod.Invoke(null, [errors])!;
    }
}
