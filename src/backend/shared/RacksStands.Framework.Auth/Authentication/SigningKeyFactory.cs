using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace RacksStands.Framework.Auth.Authentication;

public sealed class SigningKeyFactory : ISigningKeyFactory, IDisposable
{
    private readonly JwtOptions _options;
    private readonly SigningKey _currentKey;
    private readonly IReadOnlyList<SigningKey> _allKeys;
    private readonly List<RSA> _rsaKeys = new();

    public SigningKeyFactory(JwtOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        ValidateOptions(_options);

        _allKeys = LoadKeys();
        _currentKey = _allKeys.Single(k => k.IsActive);
    }

    // ========================= PUBLIC =========================

    public SigningCredentials GetSigningCredentials()
        => new(_currentKey.Key, _currentKey.Algorithm);

    public string GetKeyId() => _currentKey.KeyId;

    public IEnumerable<SecurityKey> GetValidationKeys()
        => _allKeys.Select(k => k.Key);

    public JwksDocument GetJsonWebKeySet()
    {
        var keys = _allKeys
            .Where(k => k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow)
            .Select(k => ToJsonWebKey(k))
            .ToArray();

        return new JwksDocument(keys);
    }

    // ========================= CORE =========================

    private List<SigningKey> LoadKeys()
    {
        var keys = new List<SigningKey>();

        // 🔹 Current key
        var (securityKey, rsa) = CreateSecurityKey();
        if (rsa != null) _rsaKeys.Add(rsa);

        var keyId = ResolveKeyId(_options, securityKey);
        var algorithm = ResolveSigningAlgorithm(securityKey, _options);

        keys.Add(new SigningKey
        {
            KeyId = keyId,
            Key = securityKey,
            Algorithm = algorithm,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        // 🔹 Previous keys (for validation)
        if (_options.PreviousKeys != null)
        {
            foreach (var prev in _options.PreviousKeys)
            {
                var (key, rsaPrev) = CreateSecurityKeyFromPem(prev.Pem);
                if (rsaPrev != null) _rsaKeys.Add(rsaPrev);

                keys.Add(new SigningKey
                {
                    KeyId = prev.KeyId,
                    Key = key,
                    Algorithm = prev.Algorithm,
                    CreatedAt = prev.CreatedAt,
                    ExpiresAt = prev.ExpiresAt,
                    IsActive = false
                });
            }
        }

        return keys;
    }

    private JsonWebKey ToJsonWebKey(SigningKey key)
    {
        if (key.Key is SymmetricSecurityKey symmetric)
        {
            return new JsonWebKey
            {
                Kty = "oct",
                Kid = key.KeyId,
                K = Base64UrlEncoder.Encode(symmetric.Key),
                Use = "sig",
                Alg = key.Algorithm
            };
        }

        if (key.Key is RsaSecurityKey rsaKey)
        {
            var parameters = rsaKey.Rsa!.ExportParameters(false);

            return new JsonWebKey
            {
                Kty = "RSA",
                Kid = key.KeyId,
                Use = "sig",
                Alg = key.Algorithm,
                N = Base64UrlEncoder.Encode(parameters.Modulus),
                E = Base64UrlEncoder.Encode(parameters.Exponent)
            };
        }

        throw new NotSupportedException("Unsupported key type.");
    }

    // ========================= KEY CREATION =========================

    private (SecurityKey, RSA?) CreateSecurityKey()
    {
        if (!string.IsNullOrWhiteSpace(_options.RsaPrivateKeyPem))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(_options.RsaPrivateKeyPem);
            return (new RsaSecurityKey(rsa), rsa);
        }

        if (!string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            return (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), null);
        }

        if (_options.AutoGenerateRsaKey)
        {
            var rsa = RSA.Create(_options.RsaKeySize);
            return (new RsaSecurityKey(rsa), rsa);
        }

        throw new InvalidOperationException("No signing key configured.");
    }

    private (SecurityKey, RSA?) CreateSecurityKeyFromPem(string pem)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem);
        return (new RsaSecurityKey(rsa), rsa);
    }

    // ========================= HELPERS =========================

    private static string ResolveSigningAlgorithm(SecurityKey key, JwtOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.SigningAlgorithm))
            return options.SigningAlgorithm;

        return key switch
        {
            SymmetricSecurityKey => SecurityAlgorithms.HmacSha256,
            RsaSecurityKey => SecurityAlgorithms.RsaSha256,
            _ => throw new NotSupportedException()
        };
    }

    private static string ResolveKeyId(JwtOptions options, SecurityKey key)
    {
        if (!string.IsNullOrWhiteSpace(options.KeyId))
            return options.KeyId;

        byte[] material = key switch
        {
            SymmetricSecurityKey s => s.Key,
            RsaSecurityKey r => r.Rsa!.ExportSubjectPublicKeyInfo(),
            _ => RandomNumberGenerator.GetBytes(32)
        };

        using var sha = SHA256.Create();
        return Base64UrlEncoder.Encode(sha.ComputeHash(material)[..8]);
    }

    private static void ValidateOptions(JwtOptions options)
    {
        if (!string.IsNullOrEmpty(options.SecretKey) &&
            Encoding.UTF8.GetBytes(options.SecretKey).Length < 32)
        {
            throw new ArgumentException("SecretKey must be >= 256 bits.");
        }

        if (options.RsaKeySize < 2048)
        {
            throw new ArgumentException("RSA must be >= 2048 bits.");
        }
    }

    public void Dispose()
    {
        foreach (var rsa in _rsaKeys)
        {
            rsa.Dispose();
        }
    }
}
