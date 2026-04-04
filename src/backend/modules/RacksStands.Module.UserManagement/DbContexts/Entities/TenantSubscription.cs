using RacksStands.Module.UserManagement.DbContexts.Entities.Enums;

namespace RacksStands.Module.UserManagement.DbContexts.Entities;

internal class TenantSubscription : IEntity<string>, IEntityAudit, IEntityConcurrency
{
    public required string Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Free;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? ExpireAt { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
