using eWatson.Primitives.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Validates a request of type <typeparamref name="TRequest"/> before it reaches its handler.
/// Register implementations in DI; the <c>ValidationBehavior</c> resolves all validators
/// for a request and short-circuits the pipeline if any errors are found.
/// </summary>
/// <typeparam name="TRequest">The request type to validate.</typeparam>
public interface IRequestValidator<in TRequest>
    where TRequest : notnull
{
    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <returns>
    /// A read-only list of <see cref="ResultError"/> objects representing validation failures.
    /// Return an empty list when the request is valid.
    /// </returns>
    IReadOnlyList<ResultError> Validate(TRequest request);
}
