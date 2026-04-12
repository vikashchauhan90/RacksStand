using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Auth.Http;

public interface ICookieAccessor
{
    string? GetCookie(string key);
    void SetCookie(string key, string value, CookieOptions options);
    void DeleteCookie(string key);
}
