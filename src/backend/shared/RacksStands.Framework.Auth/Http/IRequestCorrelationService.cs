
namespace RacksStands.Framework.Auth.Http;

public interface IRequestCorrelationService
{
    string GetCorrelationId();
    void SetCorrelationId(string correlationId);
}
