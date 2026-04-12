using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Module.UserManagement.DbContexts.Repositories;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Auth.UserInfo;

internal class UserInfoHandler(
    IUserRepository userRepository,
    ITenantMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserInfoHandler> logger
) : IQueryHandler<UserInfoQuery, Outcome<UserInfoResponse>>
{
    public async Task<Outcome<UserInfoResponse>> HandleAsync(UserInfoQuery request, CancellationToken ct = default)
    {
        // Get current user ID from claims
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentTenantId = tenantContext.GetCurrentTenantId();

        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("UserInfo requested without authenticated user");
            return Outcome<UserInfoResponse>.Unauthorized(new OutcomeError(
                "UserInfo.NotAuthenticated",
                "User not authenticated."));
        }

        // Get user details
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} not found", userId);
            return Outcome<UserInfoResponse>.NotFound(new OutcomeError(
                "UserInfo.UserNotFound",
                "User not found."));
        }

        // Get roles and permissions based on tenant context
        List<string> roles = new();
        List<string> permissions = new();

        logger.LogInformation("User info retrieved for {UserName} (Tenant: {TenantId})",
            user.UserName, currentTenantId ?? "No Tenant");

        var response = new UserInfoResponse(
            user.Id,
            user.Name,
            user.UserName,
            user.Email,
            roles,
            permissions
        );

        return Outcome<UserInfoResponse>.Success(response);
    }

}
