using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultifyCore;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Auth.UserInfo;

internal class UserInfoHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserInfoHandler> logger
) : IQueryHandler<UserInfoQuery, Outcome<UserInfoResponse>>
{
    public async Task<Outcome<UserInfoResponse>> HandleAsync(UserInfoQuery request, CancellationToken ct = default)
    {
        // Get current user ID from claims
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("UserInfo requested without authenticated user");
            return Outcome<UserInfoResponse>.Unauthorized(new OutcomeError(
                "UserInfo.NotAuthenticated",
                "User not authenticated."));
        }

        // Get user details
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null, ct);

        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", userId);
            return Outcome<UserInfoResponse>.NotFound(new OutcomeError(
                "UserInfo.UserNotFound",
                "User not found."));
        }

        // Get roles
        var roles = await GetUserRoles(user.Id, ct);

        logger.LogInformation("User info retrieved for {UserName}", user.UserName);

        var response = new UserInfoResponse(
            user.Id,
            user.Name,
            user.UserName,
            user.Email,
            roles,
            new List<string>()
        );

        return Outcome<UserInfoResponse>.Success(response);
    }

    private async Task<List<string>> GetUserRoles(string userId, CancellationToken ct)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ct);
    }
}
