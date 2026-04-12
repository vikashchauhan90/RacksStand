using System;
using System.Text;

namespace RacksStands.Framework.Base.Encoders;

/// <summary>
/// Provides Base32 encoding/decoding as defined by RFC 4648 (without padding by default).
/// </summary>
public static class Base32Encoder
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
    private const char PaddingChar = '=';

    /// <summary>
    /// Encodes bytes into Base32 string (no padding).
    /// </summary>
    public static string Encode(ReadOnlySpan<byte> data)
    {
        if (data.Length == 0)
            return string.Empty;

        int outputLength = (data.Length + 4) / 5 * 8;
        Span<char> buffer = outputLength <= 256 ? stackalloc char[outputLength] : new char[outputLength];
        int written = EncodeCore(data, buffer, false);
        return new string(buffer[..written]);
    }

    /// <summary>
    /// Encodes a UTF8 string into Base32 (no padding).
    /// </summary>
    public static string Encode(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return Encode(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    /// Decodes a Base32 string (with or without padding) into a byte array.
    /// </summary>
    public static byte[] DecodeToBytes(string base32)
    {
        if (string.IsNullOrWhiteSpace(base32))
            throw new ArgumentNullException(nameof(base32));

        // Remove whitespace and convert to uppercase (alphabet is uppercase)
        var clean = base32.Trim().ToUpperInvariant();
        int paddingCount = 0;
        if (clean.Contains(PaddingChar))
        {
            paddingCount = clean.AsSpan().Slice(clean.IndexOf(PaddingChar)).Count(PaddingChar);
            clean = clean.TrimEnd(PaddingChar);
        }

        // Validate characters
        foreach (char c in clean)
        {
            if (Base32Alphabet.IndexOf(c) == -1)
                throw new ArgumentException($"Invalid Base32 character: '{c}'", nameof(base32));
        }

        int outputLength = clean.Length * 5 / 8;
        byte[] result = new byte[outputLength];
        DecodeCore(clean.AsSpan(), result);
        return result;
    }

    /// <summary>
    /// Decodes a Base32 string into a UTF8 string.
    /// </summary>
    public static string Decode(string base32)
    {
        var bytes = DecodeToBytes(base32);
        return Encoding.UTF8.GetString(bytes);
    }

    // Core encoding implementation (no padding)
    private static int EncodeCore(ReadOnlySpan<byte> input, Span<char> output, bool addPadding)
    {
        int inputIndex = 0, outputIndex = 0;
        int inputLength = input.Length;
        byte[] buffer = new byte[5];
        int bytesInBuffer = 0;

        while (inputIndex < inputLength)
        {
            buffer[bytesInBuffer++] = input[inputIndex++];
            if (bytesInBuffer == 5)
            {
                outputIndex = WriteGroup(buffer, output, outputIndex, 5, addPadding);
                bytesInBuffer = 0;
            }
        }

        if (bytesInBuffer > 0)
        {
            outputIndex = WriteGroup(buffer, output, outputIndex, bytesInBuffer, addPadding);
        }

        return outputIndex;
    }

    private static int WriteGroup(byte[] buffer, Span<char> output, int startIndex, int bytesInGroup, bool addPadding)
    {
        uint a = (uint)(bytesInGroup >= 1 ? buffer[0] : 0);
        uint b = (uint)(bytesInGroup >= 2 ? buffer[1] : 0);
        uint c = (uint)(bytesInGroup >= 3 ? buffer[2] : 0);
        uint d = (uint)(bytesInGroup >= 4 ? buffer[3] : 0);
        uint e = (uint)(bytesInGroup >= 5 ? buffer[4] : 0);

        uint combined = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;

        int outputCount = 0;
        output[startIndex + outputCount++] = Base32Alphabet[(int)(combined >> 35) & 0x1F];
        output[startIndex + outputCount++] = Base32Alphabet[(int)(combined >> 30) & 0x1F];
        output[startIndex + outputCount++] = bytesInGroup >= 2 ? Base32Alphabet[(int)(combined >> 25) & 0x1F] : (addPadding ? PaddingChar : ' ');
        output[startIndex + outputCount++] = bytesInGroup >= 2 ? Base32Alphabet[(int)(combined >> 20) & 0x1F] : (addPadding ? PaddingChar : ' ');
        output[startIndex + outputCount++] = bytesInGroup >= 3 ? Base32Alphabet[(int)(combined >> 15) & 0x1F] : (addPadding ? PaddingChar : ' ');
        output[startIndex + outputCount++] = bytesInGroup >= 3 ? Base32Alphabet[(int)(combined >> 10) & 0x1F] : (addPadding ? PaddingChar : ' ');
        output[startIndex + outputCount++] = bytesInGroup >= 4 ? Base32Alphabet[(int)(combined >> 5) & 0x1F] : (addPadding ? PaddingChar : ' ');
        output[startIndex + outputCount++] = bytesInGroup >= 5 ? Base32Alphabet[(int)combined & 0x1F] : (addPadding ? PaddingChar : ' ');

        if (!addPadding)
        {
            // Trim padding characters
            while (outputCount > 0 && output[startIndex + outputCount - 1] == ' ')
                outputCount--;
        }

        return startIndex + outputCount;
    }

    // Core decoding implementation (handles unpadded input)
    private static void DecodeCore(ReadOnlySpan<char> input, Span<byte> output)
    {
        int inputIndex = 0, outputIndex = 0;
        int buffer = 0;
        int bitsInBuffer = 0;

        while (inputIndex < input.Length)
        {
            char c = input[inputIndex++];
            int value = Base32Alphabet.IndexOf(c);
            if (value == -1) continue; // Skip whitespace (should not happen due to validation)

            buffer = (buffer << 5) | value;
            bitsInBuffer += 5;

            if (bitsInBuffer >= 8)
            {
                bitsInBuffer -= 8;
                output[outputIndex++] = (byte)(buffer >> bitsInBuffer);
                buffer &= (1 << bitsInBuffer) - 1;
            }
        }
    }
}
