using System.ComponentModel.DataAnnotations;

namespace RacksStands.Framework.Persistence.Options;

public sealed class DbOptions
{
    [Required]
    [MinLength(10)]
    public string ConnectionString { get; set; } = string.Empty;

    [Range(1, 10)]
    public int MaxRetryCount { get; set; } = 5;

    [Range(1, 60)]
    public int MaxRetryDelaySeconds { get; set; } = 15;
}
