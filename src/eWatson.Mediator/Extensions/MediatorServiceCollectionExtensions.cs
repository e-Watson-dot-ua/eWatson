using System.Reflection;
using eWatson.Mediator.Abstractions;
using eWatson.Mediator.Pipeline;

namespace Microsoft.Extensions.DependencyInjection;
/// <summary>
/// Extension methods for registering the eWatson Mediator in an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class MediatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the mediator, all handlers found in the specified assemblies,
    /// and any enabled built-in pipeline behaviours.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate to configure <see cref="MediatorOptions"/>.</param>
    /// <param name="assemblies">
    /// Assemblies to scan for <see cref="IRequestHandler{TRequest,TResponse}"/>,
    /// <see cref="INotificationHandler{TNotification}"/> and
    /// <see cref="IRequestValidator{TRequest}"/> implementations.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <remarks>
    /// Pipeline behaviour registration order (outer → inner):
    /// <list type="number">
    ///   <item><see cref="ExceptionHandlingBehavior{TRequest,TResponse}"/> (if enabled)</item>
    ///   <item><see cref="LoggingBehavior{TRequest,TResponse}"/> (if enabled)</item>
    ///   <item><see cref="ValidationBehavior{TRequest,TResponse}"/> (if enabled)</item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddEWatsonMediator(
        this IServiceCollection services,
        Action<MediatorOptions>? configure = null,
        params Assembly[] assemblies)
    {
        var options = new MediatorOptions();
        configure?.Invoke(options);
        // Register options as singleton so Mediator can consume them.
        services.AddSingleton(options);
        // Register the mediator itself as Scoped (one per DI scope / HTTP request).
        services.AddScoped<IMediator, global::eWatson.Mediator.Mediator>();
        // Scan assemblies for handlers and validators.
        foreach (var assembly in assemblies)
            RegisterFromAssembly(services, assembly);
        // Register built-in pipeline behaviours in outer-to-inner order.
        // Behaviours are stored as IEnumerable<IPipelineBehavior<,>> — the mediator
        // retrieves them in registration order and reverses to build the chain.
        if (options.EnableExceptionHandlingBehavior)
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ExceptionHandlingBehavior<,>));
        if (options.EnableLoggingBehavior)
        {
            // AddLogging() is idempotent — safe to call even when the host has
            // already configured a logging provider.
            services.AddLogging();
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(LoggingBehavior<,>));
        }
        if (options.EnableValidationBehavior)
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
        return services;
    }
    // -------------------------------------------------------------------------
    private static void RegisterFromAssembly(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });
        foreach (var type in types)
        {
            RegisterHandlers(services, type, typeof(IRequestHandler<,>));
            RegisterHandlers(services, type, typeof(INotificationHandler<>));
            RegisterHandlers(services, type, typeof(IRequestValidator<>));
        }
    }
    private static void RegisterHandlers(
        IServiceCollection services,
        Type implementationType,
        Type openGenericInterface)
    {
        var interfaces = implementationType
            .GetInterfaces()
            .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == openGenericInterface);
        foreach (var iface in interfaces)
            services.AddTransient(iface, implementationType);
    }
}
