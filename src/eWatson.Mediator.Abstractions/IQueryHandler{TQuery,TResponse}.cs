using eWatson.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Handles a query that returns <see cref="Result{TResponse}"/>.
/// Semantic alias for <see cref="IRequestHandler{TRequest,TResponse}"/> that makes
/// handler intent explicit at a glance.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The type of the value returned on success.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse> { }
