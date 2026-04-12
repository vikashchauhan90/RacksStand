using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Recovery;

public record RegenerateRecoveryCodesCommand : ICommand<Outcome<string[]>>;
