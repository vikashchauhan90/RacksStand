using Microsoft.EntityFrameworkCore;
using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Logout;

internal class LogoutHandler(
    UserManagementDbContext dbContext,
    ILogger<LogoutHandler> logger
) : ICommandHandler<LogoutCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(LogoutCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Processing logout request");

        // Find and revoke the refresh token
        var refreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == command.RefreshToken, ct);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await dbContext.SaveChangesAsync(ct);
            logger.LogInformation("Refresh token revoked for user {UserId}", refreshToken.UserId);
        }

        return Outcome<Unit>.Success(Unit.Value);
    }
}
