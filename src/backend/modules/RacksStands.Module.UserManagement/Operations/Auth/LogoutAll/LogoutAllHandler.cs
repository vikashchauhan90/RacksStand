using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RacksStands.Module.UserManagement.Operations.Auth.Logout;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Auth.LogoutAll;

internal class LogoutAllHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LogoutAllHandler> logger
) : ICommandHandler<LogoutAllCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(LogoutAllCommand request, CancellationToken ct = default)
    {
        // Get current user ID from claims
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("LogoutAll attempted without authenticated user");
            return Outcome<Unit>.Unauthorized(new OutcomeError(
                "LogoutAll.NotAuthenticated",
                "User not authenticated."));
        }

        logger.LogInformation("Revoking all refresh tokens for user {UserId}", userId);

        // Revoke all refresh tokens for this user
        var refreshTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
        }

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("Revoked {Count} refresh tokens for user {UserId}", refreshTokens.Count, userId);

        return Outcome<Unit>.Success(Unit.Value);
    }
}
