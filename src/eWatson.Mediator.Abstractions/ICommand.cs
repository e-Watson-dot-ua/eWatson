using eWatson.Results;
namespace eWatson.Mediator.Abstractions;
/// <summary>
/// Marker interface for a command that returns <see cref="Result"/>.
/// Commands express intent to mutate state; they always return a <see cref="Result"/>
/// to propagate success or structured failure without throwing exceptions.
/// </summary>
public interface ICommand : IRequest<Result>
{
}
