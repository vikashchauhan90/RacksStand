using System.Text.Json.Serialization;
using System.Xml;

namespace RacksStands.Framework.Hal;

public class Link : ILink
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public HttpVerbs Method { get; init; }

}
