using Microsoft.AspNetCore.Http;
using System.Net;


namespace RacksStands.Framework.Auth.Http;


internal sealed class ClientIpAddressResolver : IClientIpAddressResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientIpAddressResolver(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public IPAddress? GetClientIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return null;

        // Check for proxy forwarded IP (X-Forwarded-For)
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var firstIp = forwardedFor.FirstOrDefault()?.Split(',')[0].Trim();
            if (IPAddress.TryParse(firstIp, out var ip))
                return ip;
        }

        // Fallback to the direct remote IP address
        return context.Connection.RemoteIpAddress;
    }

    public string? GetClientIpString()
        => GetClientIpAddress()?.ToString();
}
