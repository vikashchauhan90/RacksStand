using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace RacksStands.Module.UserManagement.Operations.Mfa.Status;

internal sealed class MfaStatusHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : IQueryHandler<MfaStatusQuery, Outcome<MfaStatusResponse>>
{
    public async Task<Outcome<MfaStatusResponse>> HandleAsync(MfaStatusQuery request, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Outcome<MfaStatusResponse>.Unauthorized();

        var setting = await dbContext.UserMfaSettings.FirstOrDefaultAsync(s => s.UserId == userId, ct);
        if (setting == null || !setting.IsEnabled)
            return Outcome<MfaStatusResponse>.Success(new MfaStatusResponse(false, false));

        // Count unused recovery codes (if stored as hashed list – simplified)
        var recoveryCodesLeft = "5"; // Placeholder
        return Outcome<MfaStatusResponse>.Success(new MfaStatusResponse(true, true, recoveryCodesLeft));
    }
}
