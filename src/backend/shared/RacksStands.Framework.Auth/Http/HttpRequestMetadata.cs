using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Auth.Token;

namespace RacksStands.Framework.Auth.Http;

internal sealed class HttpRequestMetadata : IHttpRequestMetadata
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly IClientIpAddressResolver _ipResolver;
    private readonly IRequestCorrelationService _correlationService;
    private readonly IHttpHeaderReader _headerReader;
    private readonly ICookieAccessor _cookieAccessor; // optional, we'll implement if needed

    public HttpRequestMetadata(
        IHttpContextAccessor httpContextAccessor,
        IAccessTokenProvider accessTokenProvider,
        IClientIpAddressResolver ipResolver,
        IRequestCorrelationService correlationService,
        IHttpHeaderReader headerReader,
        ICookieAccessor cookieAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _accessTokenProvider = accessTokenProvider;
        _ipResolver = ipResolver;
        _correlationService = correlationService;
        _headerReader = headerReader;
        _cookieAccessor = cookieAccessor;
    }

    public string Method
        => _httpContextAccessor.HttpContext?.Request.Method ?? "UNKNOWN";

    public string Path
        => _httpContextAccessor.HttpContext?.Request.Path ?? string.Empty;

    public string? QueryString
        => _httpContextAccessor.HttpContext?.Request.QueryString.ToString();

    public string? AccessToken
        => _accessTokenProvider.GetAccessToken();

    public string? ClientIp
        => _ipResolver.GetClientIpString();

    public string? UserAgent
        => _headerReader.GetHeader("User-Agent");

    public string? CorrelationId
        => _correlationService.GetCorrelationId();

    public IReadOnlyDictionary<string, string?> Headers
        => _headerReader.GetAllHeaders();

    public IReadOnlyDictionary<string, string?> Cookies
    {
        get
        {
            var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
            if (cookies == null)
                return new Dictionary<string, string?>();

            return cookies.ToDictionary(c => c.Key, c => (string?)c.Value);
        }
    }
}
