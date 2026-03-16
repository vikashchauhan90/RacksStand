namespace RacksStands.Framework.Hal.Builders;

public interface ILinkBuilder
{
    ILink Build();
    ILinkBuilder SetHref(string href);
    ILinkBuilder SetMethod(HttpVerbs method);
    ILinkBuilder SetRel(string rel);
}
