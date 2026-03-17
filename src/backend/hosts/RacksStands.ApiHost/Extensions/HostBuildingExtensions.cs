using Asp.Versioning;
using RacksStands.Framework.Auth;
using RacksStands.Framework.Auth.Authorization;
using RacksStands.Framework.Cqrs;
using RacksStands.Framework.Modules.Bootstrap.Extensions;
using RacksStands.Framework.Monitoring.Extensions;

namespace RacksStands.ApiHost.Extensions;

public static class HostBuildingExtensions
{
    public static WebApplication AddServices(this WebApplicationBuilder builder)
    {
        // Core framework
        builder.Services.AddOptions();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();
        // JSON stays available because System.Text.Json is always included by default
        builder.Services.AddControllers()
            .AddXmlSerializerFormatters(); // adds XML

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Add monitoring
        builder.AddRacksStandsMonitoring("RacksStands.ApiHost", "1.0.0");

        // Add shared frameworks
        builder.Services.AddCqrs();


        // Add auth
        builder.Services.AddJwtAuthentication("your-issuer", "your-audience", "your-secret-key");
        builder.Services.AddDummyAuthorization();

        // Load modules dynamically
        builder.Services.AddModules(
            builder.Configuration,
            typeof(RacksStands.Module.UserManagement.UserManagementModule)
            );

        builder.Services.AddOpenApi();
        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddHealthChecks();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseRequestScope();
        // Global exception handler should be first
        app.UseGlobalExceptionHandler();
        app.UseCorrelationId();
        app.UseCors();
        // Security headers
        app.UseSecurityHeaders();
        // Idempotency middleware
        app.UseIdempotency();
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAuthenticationScope();
        // Configure modules
        app.UseModules();

        app.MapHealthChecks("/health");

        return app;
    }
}
