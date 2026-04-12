using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RacksStands.Module.UserManagement.DbContexts.Repositories;


namespace RacksStands.Module.UserManagement;

public class UserManagementModule : ModuleBase
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Bind and validate options
        services.AddOptions<DbOptions>()
            .Bind(configuration.GetSection(ConfigurationSections.ModuleName))
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
        }, lifetime: ServiceLifetime.Scoped);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantMembershipRepository, TenantMembershipRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IMagicLinkTokenRepository, MagicLinkTokenRepository>();
        services.AddScoped<IMfaChallengeRepository, MfaChallengeRepository>();
        services.AddScoped<IUserMfaSettingRepository, UserMfaSettingRepository>();
        services.AddScoped<IM2MClientRepository, M2MClientRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ITenantInvitationRepository, TenantInvitationRepository>();
        services.AddScoped<ITenantSubscriptionRepository, TenantSubscriptionRepository>();

    }
}
