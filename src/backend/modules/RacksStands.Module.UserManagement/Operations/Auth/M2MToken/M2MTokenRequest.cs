
namespace RacksStands.Module.UserManagement.Operations.Auth.M2MToken;

public record M2MTokenRequest(string ClientId, string ClientSecret, string? Scope);
