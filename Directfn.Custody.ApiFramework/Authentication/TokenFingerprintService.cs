using System.Security.Cryptography;
using System.Text;

namespace Directfn.Custody.ApiFramework.Authentication;

public sealed class TokenFingerprintService : ITokenFingerprintService
{
    public TokenFingerprintResult Generate()
    {
        var fingerprint = GenerateSecureRandomValue();
        var fingerprintHash = Hash(fingerprint);

        return new TokenFingerprintResult
        {
            Fingerprint = fingerprint,
            FingerprintHash = fingerprintHash
        };
    }

    public string Hash(string fingerprint)
    {
        if (string.IsNullOrWhiteSpace(fingerprint))
        {
            throw new ArgumentException("Fingerprint cannot be empty.", nameof(fingerprint));
        }

        var bytes = Encoding.UTF8.GetBytes(fingerprint);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToBase64String(hashBytes);
    }

    private static string GenerateSecureRandomValue()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }
}