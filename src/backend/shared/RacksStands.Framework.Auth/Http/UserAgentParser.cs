using Microsoft.AspNetCore.Http;
using UAParser;
using UAParser.Objects;

namespace RacksStands.Framework.Auth.Http;




internal sealed class UserAgentParser : IUserAgentParser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Lazy<ClientInfo?> _cachedClientInfo;

    public UserAgentParser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _cachedClientInfo = new Lazy<ClientInfo?>(ParseUserAgent);
    }

    public string RawUserAgent
        => _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? string.Empty;

    public string? Device
        => _cachedClientInfo.Value?.Device.Family;

    public bool IsBot
    {
        get
        {
            var ua = RawUserAgent;
            return string.IsNullOrEmpty(ua) ||
                   ua.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
                   ua.Contains("crawler", StringComparison.OrdinalIgnoreCase) ||
                   ua.Contains("spider", StringComparison.OrdinalIgnoreCase);
        }
    }

    public string? BrowserName
        => _cachedClientInfo.Value?.Browser?.Family;

    private ClientInfo? ParseUserAgent()
    {
        var ua = RawUserAgent;
        if (string.IsNullOrEmpty(ua))
            return null;

        return Parser.GetDefault().Parse(ua);
    }
}
