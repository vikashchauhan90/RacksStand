using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RacksStands.Framework.Modules.Bootstrap.Abstractions;

public abstract class ModuleBase : IModule
{
    public virtual string Name => GetType().Name;
    public virtual int Order => 0;
    public virtual string RoutePrefix => "v1";
    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    public virtual void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var moduleAssembly = GetType().Assembly;
        var group = routes.MapGroup(RoutePrefix).WithTags(Name);
        // Retrieve all IEndpoint services from the DI container
        var endpoints = routes.ServiceProvider.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
        {
            // Only map endpoints that belong to this module's assembly
            if (endpoint.GetType().Assembly == moduleAssembly)
            {
                endpoint.Map(group);
            }
        }
    }
}
