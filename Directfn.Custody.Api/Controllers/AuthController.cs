using Asp.Versioning;
using Directfn.Custody.Api.Requests.User;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.Api.Controllers
{
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public sealed class AuthController : CustodyControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthOptions _authOptions;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITokenFingerprintService _tokenFingerprintService;
        public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, ITokenFingerprintService tokenFingerprintService, IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _tokenFingerprintService = tokenFingerprintService;
            _authOptions = authOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            LoginUserRecord? user = await _userRepository.GetUserForLoginAsync(request.LoginId, request.Rf48Code, cancellationToken);
            if (user is null)
            {
                return Success(new { Found = false, Message = "User was not found." });
            }
            else 
            {
                TokenFingerprintResult fingerprint = _tokenFingerprintService.Generate();

                SetFingerprintCookie(fingerprint.Fingerprint);

                TokenResult token = _jwtTokenService.GenerateAccessToken(new JwtTokenRequest
                {
                    UserId = user.Um02Id.ToString(),
                    UserName = user.Um02LoginId,
                    SessionId = Guid.NewGuid().ToString("N"),
                    FingerprintHash = fingerprint.FingerprintHash,
                    Email = user.Um02Email,
                    Roles = ["CUSTODY_ADMIN"]
                });

                return Success(token);
            }
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
