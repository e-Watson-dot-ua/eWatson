using eWatson.Primitives.Results;

namespace eWatson.Mediator.Abstractions;

/// <summary>
/// Marker interface for a command that returns <see cref="Result{TValue}"/> on success.
/// </summary>
/// <typeparam name="TValue">
/// The domain payload carried on success. The full response type is <see cref="Result{TValue}"/>.
/// </typeparam>
public interface ICommand<TValue> : IRequest<Result<TValue>> { }
