namespace RacksStands.Module.UserManagement.Operations.Auth.M2MToken;

public record M2MTokenCommand(string ClientId, string ClientSecret, string? Scope)
    : ICommand<Outcome<M2MTokenResponse>>;
