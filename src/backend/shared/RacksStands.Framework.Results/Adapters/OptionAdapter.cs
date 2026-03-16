namespace RacksStands.Framework.Results.Adapters;

public sealed class OptionAdapter<T> : IResult
{
    private readonly Option<T> _option;

    public OptionAdapter(Option<T> option)
    {
        _option = option;
    }

    public ResultState Status =>
        Option.IsSome(_option) ? ResultState.Success : ResultState.NoContent;

    public IReadOnlyDictionary<string, object> Errors =>
        Option.IsSome(_option)
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>
            {
                ["Option.None"] = "No value present in Option."
            };
}
