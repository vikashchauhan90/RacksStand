using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Auth.Http;

internal sealed class CookieAccessor : ICookieAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CookieAccessor(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public string? GetCookie(string key)
        => _httpContextAccessor.HttpContext?.Request.Cookies[key];

    public void SetCookie(string key, string value, CookieOptions options)
        => _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);

    public void DeleteCookie(string key)
        => _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
}
