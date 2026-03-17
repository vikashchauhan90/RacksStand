namespace RacksStands.Framework.Base.Serializers;

    public static class ContentTypes
    {
        // Common MIME types
        public const string ApplicationJson = "application/json";
        public const string ApplicationJsonUtf8 = "application/json; charset=utf-8";

        public const string ApplicationXml = "application/xml";
        public const string ApplicationXmlUtf8 = "application/xml; charset=utf-8";

        public const string TextXml = "text/xml";
        public const string TextXmlUtf8 = "text/xml; charset=utf-8";

        public const string TextPlain = "text/plain";
        public const string TextPlainUtf8 = "text/plain; charset=utf-8";
        public const string Anything = "*/*";

        // Supported content types collection
        public static readonly IReadOnlyCollection<string> Supported = new[]
        {
            ApplicationJson,
            ApplicationJsonUtf8,
            ApplicationXml,
            ApplicationXmlUtf8,
            TextXml,
            TextXmlUtf8,
            TextPlain,
            TextPlainUtf8
        };

        public static ContentType Resolve(string rawHeaderValue)
        {
            if (string.IsNullOrWhiteSpace(rawHeaderValue))
                throw new ArgumentException("Header value cannot be null or empty.", nameof(rawHeaderValue));

            // Normalize: lowercase, trim, remove parameters like "; charset=utf-8"
            var normalized = rawHeaderValue.Split(';')[0].Trim().ToLowerInvariant();

            return normalized switch
            {
                ApplicationJson => ContentType.Json,
                ApplicationXml => ContentType.Xml,
                TextXml => ContentType.Xml,
                TextPlain => ContentType.Text,
                Anything => ContentType.Json,
                _ => throw new NotSupportedException($"Unsupported content type: {rawHeaderValue}")
            };
        }

        public static string ToMime(ContentType type) => type switch
        {
            ContentType.Json => ApplicationJsonUtf8,
            ContentType.Xml => ApplicationXmlUtf8,
            ContentType.Text => TextPlainUtf8,
            _ => throw new NotSupportedException($"Unsupported content type: {type}")
        };
    }
