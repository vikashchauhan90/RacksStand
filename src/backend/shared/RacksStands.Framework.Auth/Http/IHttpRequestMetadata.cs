
namespace RacksStands.Framework.Auth.Http;

public interface IHttpRequestMetadata
{
    string Method { get; }
    string Path { get; }
    string? QueryString { get; }
    string? AccessToken { get; }
    string? ClientIp { get; }
    string? UserAgent { get; }
    string? CorrelationId { get; }
    IReadOnlyDictionary<string, string?> Headers { get; }
    IReadOnlyDictionary<string, string?> Cookies { get; }
}
