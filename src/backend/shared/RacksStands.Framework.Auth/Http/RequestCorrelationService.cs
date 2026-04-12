using Microsoft.AspNetCore.Http;

namespace RacksStands.Framework.Auth.Http;

internal sealed class RequestCorrelationService : IRequestCorrelationService
{
    private const string CorrelationHeaderName = "X-Correlation-ID";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestCorrelationService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string GetCorrelationId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return GenerateNewCorrelationId();

        // Try to get from request headers
        if (context.Request.Headers.TryGetValue(CorrelationHeaderName, out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            var correlationId = headerValue.ToString();
            // Store in items for the lifetime of the request (avoids re-reading headers)
            context.Items[CorrelationHeaderName] = correlationId;
            return correlationId;
        }

        // Check if already generated and stored in items
        if (context.Items.TryGetValue(CorrelationHeaderName, out var stored) && stored is string existing)
            return existing;

        // Generate new correlation ID
        var newId = GenerateNewCorrelationId();
        context.Items[CorrelationHeaderName] = newId;
        return newId;
    }

    public void SetCorrelationId(string correlationId)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return;

        context.Items[CorrelationHeaderName] = correlationId;
        // Also add to response headers so downstream services can use it
        context.Response.Headers[CorrelationHeaderName] = correlationId;
    }

    private static string GenerateNewCorrelationId()
        => Guid.NewGuid().ToString("N");
}
