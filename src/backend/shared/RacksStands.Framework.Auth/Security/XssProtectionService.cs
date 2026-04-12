// Framework/Auth/Security/XssProtectionService.cs
using System.Text;
using System.Text.RegularExpressions;

namespace RacksStands.Framework.Auth.Security;

internal sealed class XssProtectionService : IXssProtectionService
{
    private static readonly Regex _dangerousTagsRegex = new(
        @"<(script|iframe|object|embed|link|style|meta|form|input|button|textarea|select|option|applet|base|body|head|html)[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex _dangerousAttributesRegex = new(
        @"\s(on\w+)\s*=|javascript:|data:text/html|vbscript:|expression\(|eval\(|alert\(",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex _dangerousProtocolsRegex = new(
        @"^(javascript|vbscript|data|file|about):",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly char[] _htmlSpecialChars = { '<', '>', '&', '"', '\'' };

    public string SanitizeHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var sanitized = html;

        // Remove dangerous tags
        sanitized = _dangerousTagsRegex.Replace(sanitized, "");

        // Remove dangerous attributes
        sanitized = _dangerousAttributesRegex.Replace(sanitized, "");

        // Encode remaining HTML special characters
        sanitized = HtmlEncode(sanitized);

        return sanitized;
    }

    public string SanitizeInput(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Encode dangerous characters
        var sanitized = HtmlEncode(input);

        return sanitized;
    }

    public bool ContainsXssRisk(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        return _dangerousTagsRegex.IsMatch(input) ||
               _dangerousAttributesRegex.IsMatch(input) ||
               _dangerousProtocolsRegex.IsMatch(input);
    }

    public string SanitizeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return "#";

        // Check for dangerous protocols
        if (_dangerousProtocolsRegex.IsMatch(url))
            return "#";

        // Remove dangerous characters
        var sanitized = url.Replace("'", "").Replace("\"", "").Replace("<", "").Replace(">", "");

        return sanitized;
    }

    public string HtmlAttributeEncode(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            switch (c)
            {
                case '&': sb.Append("&amp;"); break;
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '"': sb.Append("&quot;"); break;
                case '\'': sb.Append("&#39;"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }

    public string JavaScriptEncode(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\'': sb.Append("\\'"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '<': sb.Append("\\x3C"); break;
                case '>': sb.Append("\\x3E"); break;
                case '&': sb.Append("\\x26"); break;
                default:
                    if (c < 0x20 || c > 0x7F)
                        sb.AppendFormat("\\u{0:X4}", (int)c);
                    else
                        sb.Append(c);
                    break;
            }
        }
        return sb.ToString();
    }

    private static string HtmlEncode(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Check if encoding is needed
        if (input.IndexOfAny(_htmlSpecialChars) == -1)
            return input;

        var sb = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            switch (c)
            {
                case '&': sb.Append("&amp;"); break;
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '"': sb.Append("&quot;"); break;
                case '\'': sb.Append("&#39;"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }
}
