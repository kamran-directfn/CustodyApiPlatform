using Asp.Versioning;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.SampleApi.Controllers
{
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth-test")]
    public sealed class AuthTestController : CustodyControllerBase
    {
        private readonly AuthOptions _authOptions;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITokenFingerprintService _tokenFingerprintService;

        public AuthTestController(IJwtTokenService jwtTokenService, ITokenFingerprintService tokenFingerprintService, IOptions<AuthOptions> authOptions)
        {
            _jwtTokenService = jwtTokenService;
            _tokenFingerprintService = tokenFingerprintService;
            _authOptions = authOptions.Value;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken()
        {
            TokenFingerprintResult fingerprint = _tokenFingerprintService.Generate();

            SetFingerprintCookie(fingerprint.Fingerprint);

            TokenResult token = _jwtTokenService.GenerateAccessToken(new JwtTokenRequest
            {
                UserId = "1001",
                UserName = "test.user",
                SessionId = Guid.NewGuid().ToString("N"),
                FingerprintHash = fingerprint.FingerprintHash,
                Email = "test.user@directfn.com",
                Roles = ["CUSTODY_ADMIN"]
            });

            return Success(token);
        }

        private void SetFingerprintCookie(string fingerprint)
        {
            Response.Cookies.Append(_authOptions.FingerprintCookieName, fingerprint, new CookieOptions
            {
                HttpOnly = true,
                Secure = _authOptions.UseSecureCookies,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours)
            });
        }
    }
}