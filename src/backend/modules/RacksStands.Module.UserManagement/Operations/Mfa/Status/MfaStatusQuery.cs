namespace RacksStands.Module.UserManagement.Operations.Mfa.Status;

public record MfaStatusQuery : IQuery<Outcome<MfaStatusResponse>>;
public record MfaStatusResponse(bool IsEnabled, bool IsConfigured, string? RecoveryCodesLeft = null);
