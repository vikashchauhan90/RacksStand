

using Microsoft.IdentityModel.Tokens;

namespace RacksStands.Framework.Auth.Authentication;

public record JwksDocument(IReadOnlyList<JsonWebKey> Keys);
