using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RacksStands.Framework.Modules.Bootstrap.Abstractions;
using RacksStands.Framework.Modules.Bootstrap.Scanner;
using System.Reflection;

namespace RacksStands.Framework.Modules.Bootstrap;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        IEnumerable<Assembly> assemblies)
    {
        var modules = AssemblyModuleScanner.Discover(assemblies);
        foreach (var module in modules)
        {
            module.ConfigureServices(services, configuration);
            services.TryAddEnumerable(ServiceDescriptor.Singleton(module));
        }
        return services;
    }

    public static IEndpointRouteBuilder UseModules(this IEndpointRouteBuilder endpoints)
    {
        var modules = endpoints.ServiceProvider.GetServices<IModule>();
        foreach (var module in modules)
        {
            module.MapEndpoints(endpoints);
        }
        return endpoints;
    }
}
