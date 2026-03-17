using RacksStands.Framework.Monitoring.Tracking;

namespace RacksStands.ApiHost.Middlewares;

public class LogScopeMiddleware(RequestDelegate next, IHostEnvironment environment, ILogger<LogScopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var correlationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();
        var scope = new Dictionary<string, object?>
        {
            ["ServiceName"] = environment.ApplicationName,
            ["CorrelationId"] = correlationId,
            ["Environment"] = environment.EnvironmentName,
            ["RequestPath"] = context.Request.Path,
            ["RequestMethod"] = context.Request.Method,
            ["RequestHost"] = context.Request.Host.Value,
            ["UserAgent"] = context.Request.Headers.UserAgent.ToString(),
            ["ClientIp"] = clientIp,
            ["PlatformOS"] = System.Runtime.InteropServices.RuntimeInformation.OSDescription
        };
        using (logger.BeginScope(scope))
        using (logger.BeginScope(new DeferredTraceScope()))
        {
            await next(context);
        }
    }
}
