using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Authentication;

public sealed class DataProtectionRefreshTokenService : IRefreshTokenService
{
    private readonly IDataProtector _protector;
    private readonly AuthOptions _authOptions;

    public DataProtectionRefreshTokenService(IDataProtectionProvider dataProtectionProvider, IOptions<AuthOptions> authOptions)
    {
        _protector = dataProtectionProvider.CreateProtector("Directfn.Custody.RefreshToken.v1");
        _authOptions = authOptions.Value;
    }

    public string GenerateRefreshToken(JwtTokenRequest request)
    {
        var payload = new RefreshTokenPayload
        {
            UserId = request.UserId,
            UserName = request.UserName,
            SessionId = request.SessionId,
            Email = request.Email,
            FingerprintHash = request.FingerprintHash,
            Roles = request.Roles,
            ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours)
        };

        var json = JsonSerializer.Serialize(payload);
        return _protector.Protect(json);
    }

    public RefreshTokenPayload? ValidateRefreshToken(string refreshToken)
    {
        try
        {
            var json = _protector.Unprotect(refreshToken);
            var payload = JsonSerializer.Deserialize<RefreshTokenPayload>(json);

            if (payload is null)
            {
                return null;
            }

            if (payload.ExpiresAtUtc <= DateTimeOffset.UtcNow)
            {
                return null;
            }

            return payload;
        }
        catch
        {
            return null;
        }
    }
}