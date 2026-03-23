using eWatson.Primitives.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Handles a command that returns <see cref="Result{TValue}"/> on success.
/// Semantic alias for <see cref="IRequestHandler{TRequest,TResponse}"/> that makes
/// handler intent explicit at a glance.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TValue">The domain payload type carried on success.</typeparam>
public interface ICommandHandler<in TCommand, TValue> : IRequestHandler<TCommand, Result<TValue>>
    where TCommand : ICommand<TValue> { }
