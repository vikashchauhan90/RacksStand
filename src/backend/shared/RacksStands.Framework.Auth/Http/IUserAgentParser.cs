using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Auth.Http;

public interface IUserAgentParser
{
    string RawUserAgent { get; }
    bool IsMobile { get; }
    bool IsBot { get; }
    string? BrowserName { get; }
}
