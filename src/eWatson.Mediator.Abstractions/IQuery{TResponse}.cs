using eWatson.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Marker interface for a query that returns <see cref="Result{TResponse}"/>.
/// Queries are strictly read-only operations and must not cause observable side-effects.
/// </summary>
/// <typeparam name="TResponse">The type of the value returned on success.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
