namespace RacksStands.Module.UserManagement.Operations.Auth.M2MToken;

public record M2MTokenResponse(string AccessToken, int ExpiresIn, string TokenType);
