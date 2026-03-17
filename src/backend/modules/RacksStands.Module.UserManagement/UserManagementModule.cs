using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace RacksStands.Module.UserManagement;

public class UserManagementModule : IModule
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Bind and validate options
        services.AddOptions<DbOptions>()
            .Bind(configuration.GetSection(ConfigurationSections.UserManagementDb))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<AuditEntityInterceptor>();
        services.AddScoped<SecurityEntityInterceptor>();

        // DbContext registration
        services.AddDbContext<UserManagementDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DbOptions>>().Value;

            options.UseNpgsql(dbOptions.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: dbOptions.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds),
                    errorCodesToAdd: null);
            });

            options.UseSnakeCaseNamingConvention();

            options.AddInterceptors(
                sp.GetRequiredService<AuditEntityInterceptor>(),
                sp.GetRequiredService<SecurityEntityInterceptor>());
        });


        // DbContext factory registration
        services.AddDbContextFactory<UserManagementDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<DbOptions>>().Value;

            options.UseNpgsql(dbOptions.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: dbOptions.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds),
                    errorCodesToAdd: null);
            });

            options.UseSnakeCaseNamingConvention();

            options.AddInterceptors(
                sp.GetRequiredService<AuditEntityInterceptor>(),
                sp.GetRequiredService<SecurityEntityInterceptor>());
        });

    }

    public void MapEndpoints(IEndpointRouteBuilder routes)
    {
        using var scope = routes.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        dbContext.Database.Migrate(); // Apply migrations

        routes.MapUserEndpoints();
    }
}
