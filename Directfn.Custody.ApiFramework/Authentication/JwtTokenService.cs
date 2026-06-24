using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Directfn.Custody.ApiFramework.Authentication
{
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

            SigningCredentials signingCredentials = CreateSigningCredentials();

            DateTime now = DateTime.UtcNow;
            DateTime expiresAt = now.AddMinutes(_options.AccessTokenMinutes);
            string jwtId = Guid.NewGuid().ToString("N");

            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.UserId),
                new Claim(ClaimTypes.NameIdentifier, request.UserId),
                new Claim(JwtRegisteredClaimNames.UniqueName, request.UserName),
                new Claim(ClaimTypes.Name, request.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim("sid", request.SessionId),
                new Claim("session_id", request.SessionId),
                new Claim("fp_hash", request.FingerprintHash)
            };

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, request.Email));
                claims.Add(new Claim(ClaimTypes.Email, request.Email));
            }

            if (!string.IsNullOrWhiteSpace(request.MemberCode))
            {
                claims.Add(new Claim("member_code", request.MemberCode));
            }

            if (!string.IsNullOrWhiteSpace(request.MemberCodeId))
            {
                claims.Add(new Claim("member_code_id", request.MemberCodeId));
            }

            foreach (string role in request.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            JwtSecurityToken token = new(_options.Issuer, _options.Audience, claims, now, expiresAt, signingCredentials);

            string? accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResult { AccessToken = accessToken, ExpiresAtUtc = expiresAt, ExpiresInSeconds = (int)(expiresAt - now).TotalSeconds };
        }

        private SigningCredentials CreateSigningCredentials()
        {
            if (!string.IsNullOrWhiteSpace(_options.SigningKey))
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(_options.SigningKey);

                if (keyBytes.Length < 32)
                {
                    throw new InvalidOperationException("Authentication:SigningKey must be at least 32 bytes for development signing.");
                }

                SymmetricSecurityKey securityKey = new(keyBytes);

                return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            }

            throw new InvalidOperationException("No JWT signing method configured. Provide development SigningKey or production certificate.");
        }
    }
}
