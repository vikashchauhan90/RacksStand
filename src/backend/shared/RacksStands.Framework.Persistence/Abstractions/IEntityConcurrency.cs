namespace RacksStands.Framework.Persistence.Abstractions;

public interface IEntityConcurrency: IEntity
{
    public string? ConcurrencyStamp { get; set; }
}
