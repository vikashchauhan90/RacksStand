
namespace RacksStands.Module.UserManagement.Entities;

internal class RefreshToken : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTimeOffset ExpireAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
