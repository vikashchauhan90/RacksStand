namespace RacksStands.Module.UserManagement.Entities;

internal class Product : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
