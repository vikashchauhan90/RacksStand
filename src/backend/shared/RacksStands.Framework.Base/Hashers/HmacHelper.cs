
using System.Security.Cryptography;
using System.Text;

namespace RacksStands.Framework.Base.Hashers;

public static class HmacHelper
{
    public static string ComputeHash(string message, string secretKey)
    {
        if (string.IsNullOrEmpty(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("Secret key cannot be null or empty.", nameof(secretKey));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hashBytes);
    }
    public static bool VerifyHash(string message, string secretKey, string expectedHash)
    {
        var actualHash = ComputeHash(message, secretKey);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(actualHash),
            Encoding.UTF8.GetBytes(expectedHash)
        );
    }
}
