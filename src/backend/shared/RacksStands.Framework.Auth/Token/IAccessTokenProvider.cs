namespace RacksStands.Framework.Auth.Token;

/// <summary>
/// Provides access to the current HTTP request's bearer token.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Returns the raw access token from the Authorization header (Bearer scheme).
    /// Returns null if header is missing or not a valid bearer token.
    /// </summary>
    string? GetAccessToken();
}
