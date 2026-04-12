namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

public record SigninResponse(
    string AccessToken,
    int ExpiresIn,
    string RefreshToken,
    bool MfaRequired,
    string? MfaChallengeId = null);
