using RacksStands.Module.UserManagement.Operations.Auth.Signin;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Verify;
public record VerifyMfaCommand(string ChallengeId, string Code) : ICommand<Outcome<SigninResponse>>;
