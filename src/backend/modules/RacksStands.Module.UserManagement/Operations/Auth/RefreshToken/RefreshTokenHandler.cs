using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Repositories;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Auth.RefreshToken;

internal class RefreshTokenHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITenantMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    ILogger<RefreshTokenHandler> logger
) : ICommandHandler<RefreshTokenCommand, Outcome<RefreshTokenResponse>>
{
    public async Task<Outcome<RefreshTokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting to refresh token");

        // Hash the incoming refresh token for comparison
        var tokenHash = HashHelper.SHA256(command.RefreshToken);

        // Find valid refresh token
        var refreshToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        if (refreshToken == null)
        {
            logger.LogWarning("Invalid refresh token used");
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.Invalid",
                "Invalid or expired refresh token."));
        }

        // Check if token is revoked or expired
        if (refreshToken.IsRevoked)
        {
            logger.LogWarning("Revoked refresh token used for user {UserId}", refreshToken.UserId);
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.Revoked",
                "Refresh token has been revoked."));
        }

        if (refreshToken.ExpireAt <= DateTimeOffset.UtcNow)
        {
            logger.LogWarning("Expired refresh token used for user {UserId}", refreshToken.UserId);
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.Expired",
                "Refresh token has expired."));
        }

        // Get user
        var user = await userRepository.GetByIdAsync(refreshToken.UserId, ct);
        if (user == null || user.DeletedAt != null)
        {
            logger.LogWarning("User {UserId} not found or deactivated for refresh token", refreshToken.UserId);
            return Outcome<RefreshTokenResponse>.Unauthorized(new OutcomeError(
                "RefreshToken.UserNotFound",
                "User not found or deactivated."));
        }

        // Revoke the used refresh token (token rotation)
        await refreshTokenRepository.RevokeAsync(refreshToken, ct);
        logger.LogInformation("Revoked used refresh token for user {UserId}", user.Id);

        // Get user's roles based on current tenant context
        var currentTenantId = tenantContext.GetCurrentTenantId();
        List<string> roles = new();

        if (!string.IsNullOrEmpty(currentTenantId))
        {
            // Get roles for current tenant
            //roles = await roleRepository.GetRoleNamesByUserAsync(user.Id, currentTenantId, ct);
            //logger.LogInformation("User {UserId} has {RoleCount} roles in tenant {TenantId}",
            //    user.Id, roles.Count, currentTenantId);
        }
        else
        {
            // Get all unique roles across all tenants
            var memberships = await membershipRepository.GetMembershipsByUserAsync(user.Id, ct);
            var roleIds = memberships.Select(m => m.RoleId).Distinct();
            foreach (var roleId in roleIds)
            {
                var role = await roleRepository.GetByIdAsync(roleId, ct);
                if (role != null && !roles.Contains(role.Name))
                    roles.Add(role.Name);
            }
            logger.LogInformation("User {UserId} has {RoleCount} total roles across all tenants",
                user.Id, roles.Count);
        }

        // Generate new tokens
        var newAccessToken = GenerateTokenWithTenant(user, roles, currentTenantId);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = HashHelper.SHA256(newRefreshToken);

        // Store new refresh token
        var newRefreshTokenEntity = new DbContexts.Entities.RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };

        await refreshTokenRepository.AddAsync(newRefreshTokenEntity, ct);
        logger.LogInformation("New refresh token created for user {UserId}", user.Id);

        logger.LogInformation("Tokens refreshed successfully for user {UserName}", user.UserName);

        var response = new RefreshTokenResponse(
            newAccessToken,
            newRefreshToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            "Bearer"
        );

        return Outcome<RefreshTokenResponse>.Success(response);
    }

    private string GenerateTokenWithTenant(User user, List<string> roles, string? tenantId)
    {
        // Create additional claims
        var additionalClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        if (!string.IsNullOrEmpty(tenantId))
        {
            additionalClaims.Add(new Claim("tenant_id", tenantId));
        }

        // If your IJwtTokenService supports custom claims, use it
        // Otherwise, use the standard method
        return jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
    }
}
