namespace RacksStands.Framework.Results;

public interface IResult
{
    ResultState Status { get; }
    IReadOnlyDictionary<string, object> Errors { get; }

}
public interface IResult<T>: IResult
{
    T? Data { get; }

}
