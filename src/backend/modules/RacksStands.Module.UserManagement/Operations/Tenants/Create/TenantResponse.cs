namespace RacksStands.Module.UserManagement.Operations.Tenants.Create;

public record TenantResponse(
    string TenantId,
    string TenantName,
    string TenantSlug);
