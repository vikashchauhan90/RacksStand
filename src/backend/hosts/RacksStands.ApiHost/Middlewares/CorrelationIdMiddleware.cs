namespace RacksStands.ApiHost.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Generate or reuse correlation ID
        var correlationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

        // Store in HttpContext.Items if you want to use it later in the pipeline
        context.Items["CorrelationId"] = correlationId;

        // Ensure header is set just before response is sent
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Correlation-Id"] = correlationId;
            return Task.CompletedTask;
        });

        await next(context);
    }

}
