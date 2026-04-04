namespace RacksStands.Module.UserManagement.DbContexts.Entities;

internal class MagicLinkToken: IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTimeOffset ExpireAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
