using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RacksStands.Framework.Auth.Http;

public interface IClientIpAddressResolver
{
    IPAddress? GetClientIpAddress();
    string? GetClientIpString();
}
