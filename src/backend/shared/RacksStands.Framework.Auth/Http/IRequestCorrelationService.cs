using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Auth.Http;

public interface IRequestCorrelationService
{
    string GetCorrelationId();
    void SetCorrelationId(string correlationId);
}
