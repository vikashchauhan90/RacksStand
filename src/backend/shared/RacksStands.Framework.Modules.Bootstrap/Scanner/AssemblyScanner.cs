using RacksStands.Framework.Modules.Bootstrap.Abstractions;
using System.Collections;

namespace RacksStands.Framework.Modules.Bootstrap.Scanner;

internal class AssemblyScanner(IEnumerable<Type> types) : IEnumerable<AssemblyScanResult>
{
    private static readonly Type _moduleInterfaceType = typeof(IModule); // renamed for clarity

    public IEnumerator<AssemblyScanResult> GetEnumerator() => Execute().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual IEnumerable<AssemblyScanResult> Execute()
    {
        var validTypes = GetValidTypes();
        foreach (var type in validTypes)
        {
            // Get all interfaces implemented by this type
            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                // Check if this interface is exactly IModule (not a generic or derived)
                if (@interface == _moduleInterfaceType)
                {
                    yield return new AssemblyScanResult(@interface, type);
                }
            }
        }
    }

    private IEnumerable<Type> GetValidTypes()
    {
        if (types is null) return [];
        return types.Where(t => !t.IsAbstract && !t.IsInterface && !t.IsGenericTypeDefinition);
    }
}
