using System;
using System.Security.Cryptography;
using System.Text;

namespace RacksStands.Framework.Base.Encryptions;

public static class AceEncryption
{
    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 600_000;
    private const byte Version = 1;

    private const int MaxCipherSize = 10_000_000; // 10MB safety cap

    public static string Encrypt(string plainText, string passPhrase)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("Plain text cannot be null or empty.", nameof(plainText));

        if (string.IsNullOrEmpty(passPhrase))
            throw new ArgumentException("Passphrase cannot be null or empty.", nameof(passPhrase));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        // IMPORTANT: Nonce must NEVER repeat for same key
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);

        byte[] key = DeriveKey(passPhrase, salt);
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[TagSize];

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plaintextBytes);
        }

        byte[] result = new byte[1 + SaltSize + NonceSize + TagSize + ciphertext.Length];

        int offset = 0;
        result[offset++] = Version;

        Buffer.BlockCopy(salt, 0, result, offset, SaltSize);
        offset += SaltSize;

        Buffer.BlockCopy(nonce, 0, result, offset, NonceSize);
        offset += NonceSize;

        Buffer.BlockCopy(tag, 0, result, offset, TagSize);
        offset += TagSize;

        Buffer.BlockCopy(ciphertext, 0, result, offset, ciphertext.Length);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("Cipher text cannot be null or empty.", nameof(cipherText));

        if (string.IsNullOrEmpty(passPhrase))
            throw new ArgumentException("Passphrase cannot be null or empty.", nameof(passPhrase));

        byte[] fullData;

        try
        {
            fullData = Convert.FromBase64String(cipherText);
        }
        catch (FormatException ex)
        {
            throw new CryptographicException("Invalid Base64 input.", ex);
        }

        if (fullData.Length > MaxCipherSize)
            throw new CryptographicException("Cipher text too large.");

        int minLength = 1 + SaltSize + NonceSize + TagSize;
        if (fullData.Length < minLength)
            throw new CryptographicException("Invalid cipher text.");

        int offset = 0;

        byte version = fullData[offset++];
        if (version != Version)
            throw new CryptographicException($"Unsupported encryption version: {version}");

        byte[] salt = new byte[SaltSize];
        byte[] nonce = new byte[NonceSize];
        byte[] tag = new byte[TagSize];
        int cipherLength = fullData.Length - minLength;
        byte[] ciphertext = new byte[cipherLength];

        Buffer.BlockCopy(fullData, offset, salt, 0, SaltSize);
        offset += SaltSize;

        Buffer.BlockCopy(fullData, offset, nonce, 0, NonceSize);
        offset += NonceSize;

        Buffer.BlockCopy(fullData, offset, tag, 0, TagSize);
        offset += TagSize;

        Buffer.BlockCopy(fullData, offset, ciphertext, 0, cipherLength);

        byte[] key = DeriveKey(passPhrase, salt);
        byte[] plaintextBytes = new byte[cipherLength];

        try
        {
            using var aes = new AesGcm(key, TagSize);
            aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);
        }
        catch (CryptographicException ex)
        {
            throw new CryptographicException("Decryption failed. Data may be corrupted or tampered.", ex);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
        }

        try
        {
            return Encoding.UTF8.GetString(plaintextBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(plaintextBytes);
        }
    }

    private static byte[] DeriveKey(string passPhrase, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(passPhrase),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }
}
