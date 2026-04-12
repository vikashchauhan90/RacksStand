using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Module.UserManagement.Operations.Auth.M2MToken;

internal sealed class M2MTokenHandler(
    UserManagementDbContext dbContext,
    IJwtTokenService jwtTokenService,
    ILogger<M2MTokenHandler> logger) : ICommandHandler<M2MTokenCommand, Outcome<M2MTokenResponse>>
{
    public async Task<Outcome<M2MTokenResponse>> HandleAsync(M2MTokenCommand command, CancellationToken ct)
    {
        var client = await dbContext.M2MClients
            .FirstOrDefaultAsync(c => c.ClientId == command.ClientId && c.DeletedAt == null, ct);
        if (client == null || !VerifyClientSecret(command.ClientSecret, client.ClientSecretHash))
            return Outcome<M2MTokenResponse>.Unauthorized(new OutcomeError("M2M.InvalidCredentials", "Invalid client credentials."));

        // Validate requested scopes (simplified)
        var allowedScopes = client.Scopes?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        var requestedScopes = command.Scope?.Split(' ') ?? Array.Empty<string>();
        if (!requestedScopes.All(s => allowedScopes.Contains(s)))
            return Outcome<M2MTokenResponse>.Forbidden(new OutcomeError("M2M.InvalidScope", "Requested scope not allowed."));

        // Generate token (no user context)
        var token = jwtTokenService.GenerateToken(client.Id, client.Name, requestedScopes);
        logger.LogInformation("M2M token issued for client {ClientId}", client.ClientId);

        return Outcome<M2MTokenResponse>.Success(new M2MTokenResponse(
            token,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            "Bearer"));
    }

    private bool VerifyClientSecret(string secret, string hash)
        => HmacHelper.VerifyHash(secret, "global-pepper", hash); // Use configured pepper
}
