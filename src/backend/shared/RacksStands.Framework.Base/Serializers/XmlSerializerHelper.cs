using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace RacksStands.Framework.Base.Serializers;


public static class XmlSerializerHelper
{
    private static readonly XmlWriterSettings WriterSettings = new()
    {
        Indent = false,
        OmitXmlDeclaration = false,
        Encoding = Encoding.UTF8
    };

    public static T? Deserialize<T>(ReadOnlySpan<byte> data)
    {
        using var ms = new MemoryStream(data.ToArray());
        var serializer = new XmlSerializer(typeof(T?));
        return (T?)serializer.Deserialize(ms);
    }

    public static byte[] Serialize<T>(T obj)
    {
        using var ms = new MemoryStream();
        using var writer = XmlWriter.Create(ms, WriterSettings);
        var serializer = new XmlSerializer(typeof(T?));
        serializer.Serialize(writer, obj);
        return ms.ToArray();
    }

    public static T? Deserialize<T>(string data)
    {
        using var reader = new StringReader(data);
        var serializer = new XmlSerializer(typeof(T?));
        return (T?)serializer.Deserialize(reader);
    }

    public static string SerializeString<T>(T obj)
    {
        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw, WriterSettings);
        var serializer = new XmlSerializer(typeof(T?));
        serializer.Serialize(writer, obj);
        return sw.ToString();
    }
}
