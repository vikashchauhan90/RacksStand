using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Auth.Tenant;

internal sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentTenantId()
        => _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;
}
