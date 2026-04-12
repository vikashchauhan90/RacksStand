using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Framework.Auth.Http;


/// <summary>
/// Provides typed access to HTTP request headers from the current context.
/// </summary>
public interface IHttpHeaderReader
{
    /// <summary>
    /// Gets a header value as string.
    /// </summary>
    string? GetHeader(string name);

    /// <summary>
    /// Tries to get a header value.
    /// </summary>
    bool TryGetHeader(string name, out string? value);

    /// <summary>
    /// Gets all headers as a read-only dictionary.
    /// </summary>
    IReadOnlyDictionary<string, string?> GetAllHeaders();

    /// <summary>
    /// Gets a header value parsed as a specific type (e.g., int, Guid).
    /// </summary>
    T? GetHeaderAs<T>(string name, IFormatProvider? formatProvider = null) where T : struct;
}
