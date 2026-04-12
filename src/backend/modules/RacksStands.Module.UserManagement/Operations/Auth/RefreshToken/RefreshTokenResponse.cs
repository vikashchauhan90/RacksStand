namespace RacksStands.Module.UserManagement.Operations.Auth.RefreshToken;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType
);
