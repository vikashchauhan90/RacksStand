namespace RacksStands.Framework.Auth.Http;

public interface IUserAgentParser
{
    string RawUserAgent { get; }
    string? Device { get; }
    bool IsBot { get; }
    string? BrowserName { get; }
}
