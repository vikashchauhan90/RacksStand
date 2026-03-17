using System.Collections;
using System.Diagnostics;

namespace RacksStands.Framework.Monitoring.Tracking;

public sealed class DeferredTraceScope : IEnumerable<KeyValuePair<string, object?>>
{
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        yield return new("TraceId",Activity.Current?.TraceId.ToString());
        yield return new("SpanId", Activity.Current?.SpanId.ToString());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
