
namespace RacksStands.Framework.Hal;

public interface IEmbeddedResourceCollection<out T>
{
    IEnumerable<T> Embedded { get; }
}
