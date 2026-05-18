using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Directfn.Custody.ApiFramework.Authentication;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly AuthOptions _options;

    public JwtTokenService(IOptions<AuthOptions> options)
    {
        _options = options.Value;
    }

    public TokenResult GenerateAccessToken(JwtTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(_options.Issuer))
        {
            throw new InvalidOperationException("Authentication:Issuer is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.Audience))
        {
            throw new InvalidOperationException("Authentication:Audience is missing.");
        }

        var signingCredentials = CreateSigningCredentials();

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.AccessTokenMinutes);
        var jwtId = Guid.NewGuid().ToString("N");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserId),
            new(ClaimTypes.NameIdentifier, request.UserId),

            new(JwtRegisteredClaimNames.UniqueName, request.UserName),
            new(ClaimTypes.Name, request.UserName),

            new(JwtRegisteredClaimNames.Jti, jwtId),

            new("sid", request.SessionId),
            new("session_id", request.SessionId),

            new("fp_hash", request.FingerprintHash)
        };

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, request.Email));
            claims.Add(new Claim(ClaimTypes.Email, request.Email));
        }

        foreach (var role in request.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResult
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAt,
            ExpiresInSeconds = (int)(expiresAt - now).TotalSeconds
        };
    }

    private SigningCredentials CreateSigningCredentials()
    {
        if (!string.IsNullOrWhiteSpace(_options.SigningKey))
        {
            var keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "Authentication:SigningKey must be at least 32 bytes for development signing.");
            }

            var securityKey = new SymmetricSecurityKey(keyBytes);

            return new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);
        }

        throw new InvalidOperationException(
            "No JWT signing method configured. Provide development SigningKey or production certificate.");
    }
}