using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;

internal sealed class VerifyMagicLinkHandler(
    UserManagementDbContext dbContext,
    IJwtTokenService jwtTokenService,
    ILogger<VerifyMagicLinkHandler> logger) : ICommandHandler<VerifyMagicLinkCommand, Outcome<VerifyMagicLinkResponse>>
{
    public async Task<Outcome<VerifyMagicLinkResponse>> HandleAsync(VerifyMagicLinkCommand command, CancellationToken ct)
    {
        var tokenHash = HashHelper.SHA256(command.Token);
        var magicToken = await dbContext.MagicLinkTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsUsed && !t.IsRevoked && t.ExpireAt > DateTimeOffset.UtcNow, ct);
        if (magicToken == null)
            return Outcome<VerifyMagicLinkResponse>.Problem(new OutcomeError("MagicLink.InvalidOrExpired", "Invalid or expired token."));

        var user = await dbContext.Users.FindAsync(new object[] { magicToken.UserId }, ct);
        if (user == null)
            return Outcome<VerifyMagicLinkResponse>.NotFound(new OutcomeError("MagicLink.UserNotFound", "User not found."));

        // Mark token as used
        magicToken.IsUsed = true;
        magicToken.UsedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        // Issue tokens
        var roles = await GetUserRoles(user.Id, ct);
        var accessToken = jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = HashHelper.SHA256(refreshToken);
        var refreshTokenEntity = new RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.RefreshTokens.AddAsync(refreshTokenEntity, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} authenticated via magic link", user.Id);
        return Outcome<VerifyMagicLinkResponse>.Success(new VerifyMagicLinkResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken));
    }

    private async Task<List<string>> GetUserRoles(string userId, CancellationToken ct) => /* same as in SigninHandler */;
}
