using RacksStands.Framework.Modules.Bootstrap.Abstractions;
using System.Reflection;

namespace RacksStands.Framework.Modules.Bootstrap.Scanner;

internal static class AssemblyModuleScanner
{
    public static IEnumerable<IModule> Discover(IEnumerable<Assembly> assemblies)
    {
        // Get all types from the assemblies
        var allTypes = assemblies.SelectMany(a => a.GetTypes());

        // Create scanner and enumerate results (only those implementing IModule)
        var scanner = new AssemblyScanner(allTypes);
        var moduleTypes = scanner.Select(r => r.ImplementationType); // filter by IModule already inside scanner

        var modules = moduleTypes
            .Select(t => (IModule)Activator.CreateInstance(t)!)
            .OrderBy(m => m.Order);

        return modules;
    }
}
