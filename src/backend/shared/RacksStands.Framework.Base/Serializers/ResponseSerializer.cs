namespace RacksStands.Framework.Base.Serializers;

public static class ResponseSerializer
{
    public static string SerializeString<T>(T obj, ContentType type) => type switch
    {
        ContentType.Json => JsonSerializerHelper.SerializeString(obj),
        ContentType.Xml => XmlSerializerHelper.SerializeString(obj),
        ContentType.Text => TextSerializerHelper.SerializeString(obj),
        _ => throw new NotSupportedException($"Unsupported content type: {type}")
    };

    public static byte[] Serialize<T>(T obj, ContentType type) => type switch
    {
        ContentType.Json => JsonSerializerHelper.Serialize(obj),
        ContentType.Xml => XmlSerializerHelper.Serialize(obj),
        ContentType.Text => TextSerializerHelper.Serialize(obj),
        _ => throw new NotSupportedException($"Unsupported content type: {type}")
    };

    public static T? Deserialize<T>(string data, ContentType type) => type switch
    {
        ContentType.Json => JsonSerializerHelper.Deserialize<T>(data),
        ContentType.Xml => XmlSerializerHelper.Deserialize<T>(data),
        ContentType.Text => TextSerializerHelper.Deserialize<T>(data),
        _ => throw new NotSupportedException($"Unsupported content type: {type}")
    };

    public static T? Deserialize<T>(ReadOnlySpan<byte> data, ContentType type) => type switch
    {
        ContentType.Json => JsonSerializerHelper.Deserialize<T>(data),
        ContentType.Xml => XmlSerializerHelper.Deserialize<T>(data),
        ContentType.Text => TextSerializerHelper.Deserialize<T>(data),
        _ => throw new NotSupportedException($"Unsupported content type: {type}")
    };
}
