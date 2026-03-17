namespace RacksStands.ApiHost.Middlewares;

public class AuthenticationScopeMiddleware(RequestDelegate next, ILogger<LogScopeMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var authScope = new Dictionary<string, object?>
            {
                ["Authenticated"] = true,
                ["UserName"] = context.User.Identity!.Name ?? "Unknown",
                ["AuthenticationType"] = context.User.Identity!.AuthenticationType ?? "Unknown"
            };
            using (logger.BeginScope(authScope))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}
