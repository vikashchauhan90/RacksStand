using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Module.UserManagement.Operations.Auth.UserInfo;
using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.RefreshToken;

internal class RefreshTokenHandler(
    UserManagementDbContext dbContext,
    IJwtTokenService jwtTokenService,
    ILogger<RefreshTokenHandler> logger
) : ICommandHandler<RefreshTokenCommand, Outcome<RefreshTokenResponse>>
{
    public async Task<Outcome<RefreshTokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to refresh token");

        // Find valid refresh token
        var refreshToken = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == command.RefreshToken &&
                                       !rt.IsRevoked &&
                                       rt.ExpireAt > DateTime.UtcNow, ct);

        if (refreshToken == null)
        {
            logger.LogWarning("Invalid or expired refresh token used");
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.Invalid",
                "Invalid or expired refresh token."));
        }

        var user = refreshToken.User;
        if (user == null || user.DeletedAt != null)
        {
            logger.LogWarning("User not found or deactivated for refresh token");
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.UserNotFound",
                "User not found or deactivated."));
        }

        // Revoke the used refresh token
        refreshToken.IsRevoked = true;

        // Get roles
        var roles = await GetUserRoles(user.Id, ct);

        // Generate new tokens
        var newAccessToken = jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();

        // Store new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Ulid.NewUlid().ToString(),
            UserId = user.Id,
            TokenHash = newRefreshToken,
            ExpireAt = DateTime.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTime.UtcNow
        };
        await dbContext.RefreshTokens.AddAsync(newRefreshTokenEntity, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Tokens refreshed for user {UserName}", user.UserName);

        var response = new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            "Bearer"
        );

        return Outcome<RefreshTokenResponse>.Success(response);
    }

    private async Task<List<string>> GetUserRoles(string userId, CancellationToken ct)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ct);
    }
}
