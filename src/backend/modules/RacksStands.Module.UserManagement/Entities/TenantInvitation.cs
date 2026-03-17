namespace RacksStands.Module.UserManagement.Entities;

internal class TenantInvitation : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string TenantSubscriptionId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public string? InvitedByUserId { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTimeOffset? ExpireAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? AcceptedByUserId { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
