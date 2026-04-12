// Operations/Auth/MagicLink/VerifyMagicLinkCommand.cs
using ResultifyCore;

public record VerifyMagicLinkCommand(string Token) : ICommand<Outcome<VerifyMagicLinkResponse>>;

public record VerifyMagicLinkResponse(string AccessToken, int ExpiresIn, string RefreshToken);
