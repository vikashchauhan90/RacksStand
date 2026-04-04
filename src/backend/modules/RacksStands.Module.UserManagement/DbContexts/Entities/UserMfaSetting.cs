namespace RacksStands.Module.UserManagement.DbContexts.Entities;

internal class UserMfaSetting : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string RecoveryCodeHash { get; set; } = string.Empty;
    public string TotpSecretEncrypted { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTimeOffset? LastUsedAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
