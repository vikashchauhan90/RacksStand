using Microsoft.IdentityModel.Tokens;

namespace RacksStands.Framework.Auth.Authentication;

public interface ISigningKeyFactory
{
    SigningCredentials GetSigningCredentials();
    string GetKeyId();
    IEnumerable<SecurityKey> GetValidationKeys(); 
    JwksDocument GetJsonWebKeySet();
}
