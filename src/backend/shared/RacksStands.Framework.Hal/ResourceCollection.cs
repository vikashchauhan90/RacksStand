namespace RacksStands.Framework.Hal;

public class ResourceCollection<TData> : Resource, IResourceCollection<TData>
{
    public IEnumerable<TData> Data { get; init; }
    public IDictionary<string, dynamic> Embedded { get; init; } = new Dictionary<string, dynamic>();

    public ResourceCollection(IEnumerable<TData> data)
    {
        Data = data;
    }
    public void AddEmbeddedResourceCollection<T>(string key, IEmbeddedResourceCollection<T> resource)
    {
        Embedded[key] = resource;
    }
}

public class ResourceCollection<TData, TMeta> : ResourceCollection<TData>, IResourceCollection<TData, TMeta>
{
    public TMeta Meta { get; init; }
    public ResourceCollection(IEnumerable<TData> data, TMeta meta) : base(data)
    {
        Meta = meta;
    }
}
