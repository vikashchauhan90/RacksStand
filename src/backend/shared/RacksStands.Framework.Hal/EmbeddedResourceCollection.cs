namespace RacksStands.Framework.Hal;

public class EmbeddedResourceCollection<T> : IEmbeddedResourceCollection<T>
{
    public IEnumerable<T> Embedded { get; init; }
    public EmbeddedResourceCollection(IEnumerable<T> embedded)
    {
        Embedded = embedded;
    }

}
