namespace RacksStands.Module.UserManagement.Entities;

internal class Tenant : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
