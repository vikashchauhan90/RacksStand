using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Hal;

public interface ILink
{
    string Href { get; init; }
    HttpVerbs Method { get; init; }
    string Rel { get; init; }
}
