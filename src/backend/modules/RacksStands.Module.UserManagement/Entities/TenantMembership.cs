
namespace RacksStands.Module.UserManagement.Entities;

internal class TenantMembership : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string? AssignedByUserId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
