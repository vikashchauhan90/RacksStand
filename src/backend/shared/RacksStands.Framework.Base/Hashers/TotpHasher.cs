using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using RacksStands.Framework.Base.Encoders;

namespace RacksStands.Framework.Base.Hashers;

/// <summary>
/// Time-based One-Time Password (TOTP) generator and verifier as per RFC 6238.
/// </summary>
public static class TotpHasher
{
    private const int StepSeconds = 30;      // Default step (30 seconds)
    private const int CodeDigits = 6;        // Default code length
    private const int DefaultWindow = 1;     // Default verification window (±1 step)
    private static HashAlgorithmName DefaultAlgorithm = HashAlgorithmName.SHA1;

    // Recovery code defaults
    private const int RecoveryCodeCount = 10;
    private const int RecoveryCodeLength = 10;
    private const string RecoveryCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    // ========================= PUBLIC API =========================

    /// <summary>
    /// Generates a cryptographically random Base32 secret key for TOTP.
    /// </summary>
    /// <param name="keySizeBytes">Size of the secret in bytes (recommended: 20).</param>
    /// <returns>Base32 encoded secret.</returns>
    public static string GenerateBase32Secret(int keySizeBytes = 20)
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(keySizeBytes);
        return Base32Encoder.Encode(bytes);
    }

    /// <summary>
    /// Generates a TOTP code for a given secret and time.
    /// </summary>
    public static string GenerateTotp(
        string secretBase32,
        DateTime? time = null,
        int digits = CodeDigits,
        int stepSeconds = StepSeconds,
        HashAlgorithmName algorithm = default)
    {
        if (string.IsNullOrWhiteSpace(secretBase32))
            throw new ArgumentException("Secret cannot be null or empty.", nameof(secretBase32));
        if (digits < 6 || digits > 8)
            throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 6 and 8.");
        if (stepSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(stepSeconds), "Step seconds must be positive.");

        var hashAlg = algorithm == default ? DefaultAlgorithm : algorithm;

        byte[] secretBytes;
        try
        {
            secretBytes = Base32Encoder.DecodeToBytes(secretBase32);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid Base32 secret.", nameof(secretBase32), ex);
        }

        long counter = GetTimeCounter(time ?? DateTime.UtcNow, stepSeconds);
        byte[] counterBytes = BitConverter.GetBytes(counter);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(counterBytes);

        byte[] hmacResult;
        using (var hmac = CreateHmac(hashAlg, secretBytes))
        {
            hmacResult = hmac.ComputeHash(counterBytes);
        }

        int offset = hmacResult[hmacResult.Length - 1] & 0x0F;
        int binaryCode = (hmacResult[offset] & 0x7F) << 24 |
                         (hmacResult[offset + 1] & 0xFF) << 16 |
                         (hmacResult[offset + 2] & 0xFF) << 8 |
                         (hmacResult[offset + 3] & 0xFF);

        int code = binaryCode % (int)Math.Pow(10, digits);
        return code.ToString().PadLeft(digits, '0');
    }

    /// <summary>
    /// Verifies a TOTP code against a secret, allowing a time window to compensate for clock skew.
    /// </summary>
    public static bool Verify(
        string secretBase32,
        string code,
        int window = DefaultWindow,
        DateTime? time = null,
        int digits = CodeDigits,
        int stepSeconds = StepSeconds,
        HashAlgorithmName algorithm = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        var currentTime = time ?? DateTime.UtcNow;

        for (int i = -window; i <= window; i++)
        {
            var candidateTime = currentTime.AddSeconds(i * stepSeconds);
            string generated = GenerateTotp(secretBase32, candidateTime, digits, stepSeconds, algorithm);
            if (CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(generated),
                Encoding.UTF8.GetBytes(code)))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns the number of seconds remaining in the current TOTP step.
    /// </summary>
    public static int GetRemainingSeconds(DateTime? time = null, int stepSeconds = StepSeconds)
    {
        var now = time ?? DateTime.UtcNow;
        var timestamp = new DateTimeOffset(now).ToUnixTimeSeconds();
        return (int)(stepSeconds - (timestamp % stepSeconds));
    }

    /// <summary>
    /// Generates an otpauth:// URI for use with authenticator apps (Google Authenticator, etc.).
    /// </summary>
    public static string GenerateOtpAuthUri(
        string secret,
        string email,
        string issuer,
        int digits = CodeDigits,
        int period = StepSeconds,
        HashAlgorithmName algorithm = default)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret cannot be null or empty.", nameof(secret));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("Issuer cannot be null or empty.", nameof(issuer));
        if (digits < 6 || digits > 8)
            throw new ArgumentOutOfRangeException(nameof(digits), "Digits must be between 6 and 8.");
        if (period <= 0)
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be positive.");

        var alg = algorithm == default ? DefaultAlgorithm : algorithm;
        string algName = alg.Name?.ToUpperInvariant() ?? "SHA1";

        // Build label: "Issuer:email" (URL-encoded)
        string label = Uri.EscapeDataString($"{issuer}:{email}");

        // Build parameters
        var parameters = new Dictionary<string, string>
        {
            ["secret"] = secret,
            ["issuer"] = issuer,
            ["algorithm"] = algName,
            ["digits"] = digits.ToString(),
            ["period"] = period.ToString()
        };

        var uriBuilder = new StringBuilder($"otpauth://totp/{label}?");
        foreach (var kvp in parameters)
        {
            uriBuilder.Append(Uri.EscapeDataString(kvp.Key));
            uriBuilder.Append('=');
            uriBuilder.Append(Uri.EscapeDataString(kvp.Value));
            uriBuilder.Append('&');
        }
        uriBuilder.Length--; // remove trailing '&'

        return uriBuilder.ToString();
    }

    /// <summary>
    /// Generates a set of recovery codes that can be used as backup when TOTP is unavailable.
    /// </summary>
    public static string[] GenerateRecoveryCodes(int count = RecoveryCodeCount, int codeLength = RecoveryCodeLength)
    {
        if (count <= 0 || count > 100)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 1 and 100.");
        if (codeLength < 6 || codeLength > 20)
            throw new ArgumentOutOfRangeException(nameof(codeLength), "Code length must be between 6 and 20 characters.");

        string[] codes = new string[count];
        byte[] randomBytes = new byte[codeLength];
        char[] buffer = new char[codeLength];
        int alphabetLength = RecoveryCodeAlphabet.Length;

        for (int i = 0; i < count; i++)
        {
            RandomNumberGenerator.Fill(randomBytes);
            for (int j = 0; j < codeLength; j++)
            {
                buffer[j] = RecoveryCodeAlphabet[randomBytes[j] % alphabetLength];
            }
            codes[i] = new string(buffer);
        }

        return codes;
    }

    // ========================= PRIVATE HELPERS =========================

    private static long GetTimeCounter(DateTime time, int stepSeconds)
    {
        var unixTime = new DateTimeOffset(time).ToUnixTimeSeconds();
        return unixTime / stepSeconds;
    }

    private static HMAC CreateHmac(HashAlgorithmName algorithm, byte[] key)
    {
        if (algorithm == HashAlgorithmName.SHA1)
            return new HMACSHA1(key);
        if (algorithm == HashAlgorithmName.SHA256)
            return new HMACSHA256(key);
        if (algorithm == HashAlgorithmName.SHA512)
            return new HMACSHA512(key);
        throw new ArgumentException("Unsupported HMAC algorithm. Use SHA1, SHA256, or SHA512.", nameof(algorithm));
    }
}
