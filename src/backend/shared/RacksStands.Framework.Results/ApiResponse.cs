namespace RacksStands.Framework.Results;

public record ApiResponse
{
    public bool Success { get; init; }

    public string Error { get; init; } = string.Empty;

    public IReadOnlyDictionary<string, object> ErrorMeta { get; init; } = new Dictionary<string, object>(StringComparer.Ordinal);

}


public record ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}
