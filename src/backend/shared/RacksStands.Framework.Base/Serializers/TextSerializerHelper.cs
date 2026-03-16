using System.Text;

namespace RacksStands.Framework.Base.Serializers;

public static class TextSerializerHelper
{
    public static T? Deserialize<T>(string data)
    {
        // For plain text, assume T is string or convertible
        if (typeof(T?) == typeof(string))
            return (T?)(object)data;

        throw new InvalidOperationException("Text deserialization only supports string.");
    }

    public static string SerializeString<T>(T obj) =>
        obj?.ToString() ?? string.Empty;

    public static byte[] Serialize<T>(T obj) =>
        Encoding.UTF8.GetBytes(SerializeString(obj));

    public static T? Deserialize<T>(ReadOnlySpan<byte> data) =>
        Deserialize<T>(Encoding.UTF8.GetString(data));
}

