using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Delete;
public record DeleteMfaCommand : ICommand<Outcome<Unit>>;
