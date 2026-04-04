namespace RacksStands.Framework.Modules.Bootstrap.Scanner;

internal class AssemblyScanResult(Type interfaceType, Type implementationType)
{
    /// <summary>
    /// Gets the interface type that was discovered during the assembly scan. This is the type that represents the contract for a MediatR handler, pre/post-processor, or pipeline behavior.
    /// </summary>
    public Type InterfaceType => interfaceType;

    /// <summary>
    /// Gets the implementation type that was discovered during the assembly scan. This is the concrete type that implements the interface and will be registered with the dependency injection container to handle requests, notifications, or pipeline behaviors.
    /// </summary>
    public Type ImplementationType => implementationType;
}
