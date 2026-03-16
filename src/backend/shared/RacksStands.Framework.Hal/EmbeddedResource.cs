namespace RacksStands.Framework.Hal;

public class EmbeddedResource<T> : IEmbeddedResource<T>
{
    public T Embedded { get; init; }
    public EmbeddedResource(T embedded)
    {
        Embedded = embedded;
    }
}
