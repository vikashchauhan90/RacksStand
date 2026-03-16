namespace RacksStands.Framework.Results.Adapters;

public sealed class ResultAdapter : IResult
{
    private readonly Result _result;

    public ResultAdapter(Result result) => _result = result;

    public ResultState Status => _result.Status;

    public IReadOnlyDictionary<string, object> Errors =>
        _result.Exception == null
            ? new Dictionary<string, object>()
            : new Dictionary<string, object>
            {
                ["Exception"] = _result.Exception.Message
            };
}
