using System.ComponentModel.DataAnnotations;

namespace RacksStands.Framework.Persistence.Options;

public sealed class DbOptions
{
    [Required]
    [MinLength(10)]
    public string ConnectionString { get; init; } = string.Empty;

    [Range(1, 10)]
    public int MaxRetryCount { get; init; } = 5;

    [Range(1, 60)]
    public int MaxRetryDelaySeconds { get; init; } = 15;
}
