using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Repositories;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Auth.MagicLink;

internal sealed class VerifyMagicLinkHandler(
    IMagicLinkTokenRepository magicLinkTokenRepository,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITenantMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    ILogger<VerifyMagicLinkHandler> logger
) : ICommandHandler<VerifyMagicLinkCommand, Outcome<VerifyMagicLinkResponse>>
{
    public async Task<Outcome<VerifyMagicLinkResponse>> HandleAsync(VerifyMagicLinkCommand command, CancellationToken ct)
    {
        logger.LogInformation("Verifying magic link token");

        // Hash the incoming token
        var tokenHash = HashHelper.SHA256(command.Token);

        // Find valid magic link token
        var magicToken = await magicLinkTokenRepository.GetByTokenHashAsync(tokenHash, ct);
        if (magicToken == null || magicToken.IsUsed || magicToken.IsRevoked || magicToken.ExpireAt <= DateTimeOffset.UtcNow)
        {
            logger.LogWarning("Invalid or expired magic link token used");
            return Outcome<VerifyMagicLinkResponse>.Problem(new OutcomeError(
                "MagicLink.InvalidOrExpired",
                "Invalid or expired token."));
        }

        // Get user
        var user = await userRepository.GetByIdAsync(magicToken.UserId, ct);
        if (user == null || user.DeletedAt != null)
        {
            logger.LogWarning("User {UserId} not found or deactivated for magic link", magicToken.UserId);
            return Outcome<VerifyMagicLinkResponse>.NotFound(new OutcomeError(
                "MagicLink.UserNotFound",
                "User not found."));
        }

        // Mark token as used
        await magicLinkTokenRepository.MarkUsedAsync(magicToken, ct);
        logger.LogInformation("Magic link token marked as used for user {UserId}", user.Id);

        // Determine current tenant (if any) – magic link may have tenant context
        var currentTenantId = tenantContext.GetCurrentTenantId();
        List<string> roles = new();

        if (!string.IsNullOrEmpty(currentTenantId))
        {
            // Get roles for the current tenant
           // roles = await roleRepository.GetRoleNamesByUserAsync(user.Id, currentTenantId, ct);
            logger.LogInformation("User {UserId} has {RoleCount} roles in tenant {TenantId}",
                user.Id, roles.Count, currentTenantId);
        }
        else
        {
            // If no tenant context, get all roles across all tenants
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
        var accessToken = GenerateTokenWithTenant(user, roles, currentTenantId);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = HashHelper.SHA256(refreshToken);

        var refreshTokenEntity = new DbContexts.Entities.RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };
        await refreshTokenRepository.AddAsync(refreshTokenEntity, ct);

        logger.LogInformation("User {UserId} authenticated via magic link", user.Id);

        return Outcome<VerifyMagicLinkResponse>.Success(new VerifyMagicLinkResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken));
    }

    private string GenerateTokenWithTenant(User user, List<string> roles, string? tenantId)
    {
        var additionalClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        if (!string.IsNullOrEmpty(tenantId))
        {
            additionalClaims.Add(new Claim("tenant_id", tenantId));
        }

        // If your IJwtTokenService supports custom claims, use it here.
        // Otherwise, use the standard method.
        return jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
    }
}
