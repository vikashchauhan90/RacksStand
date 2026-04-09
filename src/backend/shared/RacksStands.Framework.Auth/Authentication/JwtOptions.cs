using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace RacksStands.Framework.Auth.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(AllowEmptyStrings = false)]
    public required string Issuer { get; init; }

    [Required(AllowEmptyStrings = false)]
    public required string Audience { get; init; }

    // Symmetric key (HMAC)
    public string? SecretKey { get; init; }

    // Asymmetric keys (RSA)
    public string? RsaPrivateKeyPem { get; init; }
    public string? RsaPublicKeyPem { get; init; }

    // Key generation
    public bool AutoGenerateRsaKey { get; init; }
    public int RsaKeySize { get; init; } = 2048; // 2048, 3072, 4096

    // Key identification
    public string? KeyId { get; init; }  // Custom KID, generated if null

    // Signing algorithm
    public string SigningAlgorithm { get; init; } = SecurityAlgorithms.HmacSha256;

    public List<PreviousKeyOptions>? PreviousKeys { get; init; } = [];

    [Range(1, 1440)]
    public int AccessTokenExpireMinutes { get; init; } = 30;

    [Range(1, 30)]
    public int RefreshTokenExpireDays { get; init; } = 7;
}
