namespace RacksStands.Module.UserManagement.Operations.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Outcome<RefreshTokenResponse>>;
