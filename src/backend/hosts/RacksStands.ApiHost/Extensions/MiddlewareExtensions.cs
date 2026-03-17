using RacksStands.ApiHost.Middlewares;

namespace RacksStands.ApiHost.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
    
    public static IApplicationBuilder UseRequestScope(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LogScopeMiddleware>();
    }
    public static IApplicationBuilder UseAuthenticationScope(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationScopeMiddleware>();
    }
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
