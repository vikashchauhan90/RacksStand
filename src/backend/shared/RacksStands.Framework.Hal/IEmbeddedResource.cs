namespace RacksStands.Framework.Hal;

public interface IEmbeddedResource<out T>
{
    T Embedded { get; }
}
