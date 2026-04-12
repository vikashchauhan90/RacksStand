namespace RacksStands.Framework.Auth.Tenant;

public interface ITenantContext
{
    string? GetCurrentTenantId();
}
