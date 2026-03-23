using System.Reflection;
using eWatson.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace eWatson.Events.Extensions;

/// <summary>
/// Extension methods for registering domain event dispatching in an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class EventsServiceCollectionExtensions
{
    /// <summary>
    /// Registers the built-in <see cref="IDomainEventDispatcher"/> and scans the specified
    /// assemblies for <see cref="IDomainEventListener{TEvent}"/> implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">
    /// Assemblies to scan for <see cref="IDomainEventListener{TEvent}"/> implementations.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    public static IServiceCollection AddDomainEventDispatching(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        foreach (var assembly in assemblies)
        {
            RegisterListenersFromAssembly(services, assembly);
        }

        return services;
    }

    private static void RegisterListenersFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType
                         && i.GetGenericTypeDefinition() == typeof(IDomainEventListener<>));

            foreach (var ifc in interfaces)
            {
                services.AddTransient(ifc, type);
            }
        }
    }
}
