using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Base.IdGenerators;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Tenants.Create;

// Operations/Tenants/Create/CreateTenantHandler.cs
internal sealed class CreateTenantHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<CreateTenantCommand, Outcome<TenantResponse>>
{
    public async Task<Outcome<TenantResponse>> HandleAsync(CreateTenantCommand command, CancellationToken ct)
    {
        var ownerId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenant = new Tenant
        {
            Id = IdGenerator.NewGuidString(),
            Name = command.Name,
            Slug = command.Slug,
            OwnerId = ownerId!,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.Tenants.AddAsync(tenant, ct);

        // Add owner as admin member
        var membership = new TenantMembership
        {
            Id = IdGenerator.NewGuidString(),
            TenantId = tenant.Id,
            UserId = ownerId!,
            RoleId = "admin-role-id", // resolve actual admin role
            JoinedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.TenantMemberships.AddAsync(membership, ct);
        await dbContext.SaveChangesAsync(ct);

        return Outcome<TenantResponse>.Success(new TenantResponse(tenant.Id, tenant.Name, tenant.Slug));
    }
}
