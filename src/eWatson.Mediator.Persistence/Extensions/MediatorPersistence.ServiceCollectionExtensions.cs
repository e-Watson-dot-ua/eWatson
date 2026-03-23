using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Persistence.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Mediator.Persistence.Extensions;

/// <summary>
/// Extension methods for adding the Unit-of-Work pipeline behaviour to the
/// eWatson Mediator.
/// </summary>
public static class MediatorPersistenceServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="UnitOfWorkBehavior{TRequest,TResponse}"/> as the innermost
    /// pipeline behaviour, wrapping every command execution in a database transaction.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <remarks>
    /// <para>
    /// Call this method <em>after</em> <c>AddEWatsonMediator</c> so that the UoW behaviour
    /// is registered after the other behaviours and therefore executes closest to the handler:
    /// </para>
    /// <code>
    /// services.AddEWatsonMediator(...)
    ///         .AddEWatsonMediatorUnitOfWork();
    /// </code>
    /// <para>
    /// Ensure that an implementation of <c>IUnitOfWork</c> is registered in DI before
    /// calling this method, typically by your persistence infrastructure package.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddEWatsonMediatorUnitOfWork(
        this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        return services;
    }
}
