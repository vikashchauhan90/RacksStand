
namespace RacksStands.Framework.Persistence.Abstractions;

public interface IEntity;

public interface IEntity<TKey> : IEntity
{
    public TKey Id { get; set; }
}
