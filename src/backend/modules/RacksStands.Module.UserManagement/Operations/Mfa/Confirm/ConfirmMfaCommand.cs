using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Confirm;

public record ConfirmMfaCommand(string TotpCode, string? TotpSecret) : ICommand<Outcome<ConfirmMfaResponse>>;
public record ConfirmMfaResponse(string[] RecoveryCodes);
