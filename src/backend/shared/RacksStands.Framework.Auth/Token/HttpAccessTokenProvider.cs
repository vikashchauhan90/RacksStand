using Microsoft.AspNetCore.Http;

namespace RacksStands.Framework.Auth.Token;

internal sealed class HttpAccessTokenProvider : IAccessTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAccessTokenProvider(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string? GetAccessToken()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader))
            return null;

        const string bearerPrefix = "Bearer ";
        if (authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            return authHeader[bearerPrefix.Length..];

        return null;
    }
}
