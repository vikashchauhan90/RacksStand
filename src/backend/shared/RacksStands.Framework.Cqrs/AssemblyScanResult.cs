namespace RacksStands.Framework.Cqrs;

internal class AssemblyScanResult(Type interfaceType, Type implementationType)
{
    public Type InterfaceType => interfaceType;
    public Type ImplementationType => implementationType;
}
