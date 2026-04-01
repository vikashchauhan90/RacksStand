using System.Collections;

namespace RacksStands.Framework.Cqrs;

internal class AssemblyScanner(Type openGenericTypeDefination,IEnumerable<Type> types) : IEnumerable<AssemblyScanResult>
{
    public IEnumerator<AssemblyScanResult> GetEnumerator() => Execute().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual IEnumerable<AssemblyScanResult> Execute()
    {
        var validTypes = GetValidTypes();
        foreach (var type in validTypes)
        {
            var handlerInterfaces = type
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericTypeDefination);

            foreach (var handlerInterface in handlerInterfaces)
            {
                yield return new(handlerInterface, type);
            }
        }
    }

    private IEnumerable<Type> GetValidTypes()
    {
        if (types is null)
        {
            return [];
        }

        return types
            .Where(type => !(type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition));

    }
}
