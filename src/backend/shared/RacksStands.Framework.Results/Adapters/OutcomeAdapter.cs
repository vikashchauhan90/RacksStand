namespace RacksStands.Framework.Results.Adapters;

public sealed class OutcomeAdapter : IResult
{
    private readonly Outcome _outcome;

    public OutcomeAdapter(Outcome outcome) => _outcome = outcome;

    public ResultState Status => _outcome.Status;

    public IReadOnlyDictionary<string, object> Errors =>
        _outcome.Errors.ToDictionary(
            e => string.IsNullOrEmpty(e.Code) ? Guid.NewGuid().ToString() : e.Code,
            e => (object)e.Message
        );
}
