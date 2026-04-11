using RacksStands.Framework.Auth.Authentication;

namespace RacksStands.Module.UserManagement.Operations.Auth.Jwks;

internal class JwksHandler(
    ISigningKeyFactory signingKeyFactory,
    ILogger<JwksHandler> logger) : IQueryHandler<JwksQuery, JwksDocument>
{
    public Task<JwksDocument> HandleAsync(JwksQuery request, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching jwks keys.");
        return Task.FromResult(signingKeyFactory.GetJsonWebKeySet());
    }
}
