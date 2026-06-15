using System.Security.Cryptography;
using System.Text;

namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public static class RefreshTokenHasher
{
    public static string Hash(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be empty.", nameof(refreshToken));
        }

        byte[] bytes = Encoding.UTF8.GetBytes(refreshToken);
        byte[] hashBytes = SHA256.HashData(bytes);

        return Convert.ToBase64String(hashBytes);
    }
}