using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Delete;

// Operations/Mfa/Delete/DeleteMfaHandler.cs
internal sealed class DeleteMfaHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<DeleteMfaCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(DeleteMfaCommand request, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var setting = await dbContext.UserMfaSettings.FirstOrDefaultAsync(s => s.UserId == userId, ct);
        if (setting != null)
        {
            dbContext.UserMfaSettings.Remove(setting);
            await dbContext.SaveChangesAsync(ct);
        }
        return Outcome<Unit>.Success(Unit.Value);
    }
}
