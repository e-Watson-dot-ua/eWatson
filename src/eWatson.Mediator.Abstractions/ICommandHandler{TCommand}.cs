using eWatson.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Handles a void command that returns <see cref="Result"/>.
/// Semantic alias for <see cref="IRequestHandler{TRequest,TResponse}"/> that makes
/// handler intent explicit at a glance.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand { }
