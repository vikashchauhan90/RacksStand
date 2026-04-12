using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace RacksStands.Framework.Auth.Authentication;

/// <summary>
/// JWT Token Service implementation
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly ISigningKeyFactory _keyFactory;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(JwtOptions options, ISigningKeyFactory keyFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateToken(string userId, string userName, IEnumerable<string> roles)
    {
        var claims = BuildClaims(userId, userName, roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpireMinutes),
            SigningCredentials = _keyFactory.GetSigningCredentials(),
            IssuedAt = DateTime.UtcNow
        };

        var token = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        // Add Key ID header for JWKS support
        token.Header["kid"] = _keyFactory.GetKeyId();

        return _tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public JwksDocument GetJsonWebKeySet()
    {
        return _keyFactory.GetJsonWebKeySet();
    }

    public SecurityKey GetSecurityKey()
    {
        return _keyFactory.GetValidationKeys().First();
    }

    public SigningCredentials GetSigningCredentials()
    {
        return _keyFactory.GetSigningCredentials();
    }

    public string GetKeyId()
    {
        return _keyFactory.GetKeyId();
    }

    public int GetAccessTokenExpirySeconds()
    {
        return _options.AccessTokenExpireMinutes * 60;
    }

    public int GetRefreshTokenExpiryDays()
    {
        return _options.RefreshTokenExpireDays;
    }

    public ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = _keyFactory.GetValidationKeys(),
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private List<Claim> BuildClaims(string userId, string userName, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}
