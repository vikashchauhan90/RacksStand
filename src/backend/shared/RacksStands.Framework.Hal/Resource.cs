namespace RacksStands.Framework.Hal;

public class Resource : IResource
{
    public ISet<ILink> Links { get; init; } = new HashSet<ILink>();
    public void AddLink(ILink link)
    {
        Links.Add(link);
    }
}

public class Resource<TData> : Resource, IResource<TData>
{
    public TData Data { get; init; }

    public IDictionary<string, dynamic> Embedded { get; init; } = new Dictionary<string, dynamic>();

    public Resource(TData data)
    {
        Data = data;
    }

    public void AddEmbeddedResource<T>(string key, IEmbeddedResource<T> resource)
    {
        Embedded[key] = resource;
    }
}

public class Resource<TData, TMeta> : Resource<TData>, IResource<TData, TMeta>
{
    public TMeta Meta { get; init; }
    public Resource(TData data, TMeta meta) : base(data)
    {
        Meta = meta;
    }
}
