
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Globalization;

namespace RacksStands.Framework.Auth.Http;



internal sealed class HttpHeaderReader : IHttpHeaderReader
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpHeaderReader(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public string? GetHeader(string name)
    {
        var headers = _httpContextAccessor.HttpContext?.Request.Headers;
        return headers?.TryGetValue(name, out var values) == true ? values.ToString() : null;
    }

    public bool TryGetHeader(string name, out string? value)
    {
        value = GetHeader(name);
        return value != null;
    }

    public IReadOnlyDictionary<string, string?> GetAllHeaders()
    {
        var headers = _httpContextAccessor.HttpContext?.Request.Headers;
        if (headers == null)
            return new Dictionary<string, string?>();

        var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in headers)
        {
            dict[key] = value.ToString();
        }
        return dict;
    }

    public T? GetHeaderAs<T>(string name, IFormatProvider? formatProvider = null) where T : struct
    {
        var value = GetHeader(name);
        if (string.IsNullOrEmpty(value))
            return null;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T), formatProvider ?? CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }
}
