namespace RacksStands.Framework.Persistence.Abstractions;

public interface IEntityAudit: IEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

