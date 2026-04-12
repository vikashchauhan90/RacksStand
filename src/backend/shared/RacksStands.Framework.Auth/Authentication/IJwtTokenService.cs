using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace RacksStands.Framework.Auth.Authentication;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string userName, IEnumerable<string> roles);
    string GenerateRefreshToken();
    JwksDocument GetJsonWebKeySet();
    SecurityKey GetSecurityKey();
    SigningCredentials GetSigningCredentials();
    string GetKeyId();
    int GetAccessTokenExpirySeconds();
    int GetRefreshTokenExpiryDays();
    ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = true);
}
