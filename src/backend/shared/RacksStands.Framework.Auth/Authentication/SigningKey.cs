using Microsoft.IdentityModel.Tokens;

namespace RacksStands.Framework.Auth.Authentication;

public sealed class SigningKey
{
    public required string KeyId { get; init; }
    public required SecurityKey Key { get; init; }
    public required string Algorithm { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}

public sealed class PreviousKeyOptions
{
    public required string KeyId { get; init; }
    public required string Pem { get; init; }
    public required string Algorithm { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}
