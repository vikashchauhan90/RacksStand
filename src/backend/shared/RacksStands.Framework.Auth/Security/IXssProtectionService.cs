namespace RacksStands.Framework.Auth.Security;

/// <summary>
/// Provides XSS protection through input sanitization and validation.
/// </summary>
public interface IXssProtectionService
{
    /// <summary>
    /// Sanitizes HTML content by removing dangerous tags and attributes.
    /// </summary>
    string SanitizeHtml(string? html);

    /// <summary>
    /// Sanitizes plain text input by encoding dangerous characters.
    /// </summary>
    string SanitizeInput(string? input);

    /// <summary>
    /// Checks if input contains potential XSS risks.
    /// </summary>
    bool ContainsXssRisk(string? input);

    /// <summary>
    /// Sanitizes URL to prevent javascript: and other dangerous protocols.
    /// </summary>
    string SanitizeUrl(string? url);

    /// <summary>
    /// Encodes a string for safe use in HTML attributes.
    /// </summary>
    string HtmlAttributeEncode(string? input);

    /// <summary>
    /// Encodes a string for safe use in JavaScript strings.
    /// </summary>
    string JavaScriptEncode(string? input);
}
