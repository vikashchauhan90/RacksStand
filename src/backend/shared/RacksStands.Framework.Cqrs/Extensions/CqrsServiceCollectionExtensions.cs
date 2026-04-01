using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RacksStands.Framework.Cqrs.Abstractions;

namespace RacksStands.Framework.Cqrs.Extensions;

public static class CqrsServiceCollectionExtensions
{

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }

    public static void AddHandlersFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var handlerTypes = new AssemblyScanner(typeof(IHandler<,>),typeof(T).Assembly.GetTypes());

        foreach (var handlerType in handlerTypes)
        {
            services.TryAdd(new ServiceDescriptor(handlerType.InterfaceType, handlerType.ImplementationType, lifetime));
        }
    }

    public static void AddPipelineBehaviorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var pipelineBehaviorTypes = new AssemblyScanner(typeof(IPipelineBehavior<,>), typeof(T).Assembly.GetTypes());

        foreach (var pipelineBehaviorType in pipelineBehaviorTypes)
        {
            services.TryAdd(new ServiceDescriptor(pipelineBehaviorType.InterfaceType, pipelineBehaviorType.ImplementationType, lifetime));
        }
    }
}
