using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RacksStands.Framework.Modules.Bootstrap.Abstractions;

namespace RacksStands.Framework.Modules.Bootstrap;

public abstract class ModuleBase : IModule
{
    public virtual string Name => GetType().Name;

    public virtual int Order => 0;

    protected virtual string RoutePrefix => "v1";

    public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    public virtual void MapEndpoints(IEndpointRouteBuilder routes)
    {
        var moduleAssembly = typeof(ModuleBase).Assembly;
        var group = routes.MapGroup(RoutePrefix).WithName(Name);

        var endpoints = routes.ServiceProvider.GetServices<IEndpoint>().Where(e => e.GetType().Assembly == moduleAssembly).ToList();
        foreach (var endpoint in endpoints)
        {
            endpoint.Map(group);
        }
    }

}
