using Asp.Versioning;
using Directfn.Custody.Api.Requests.Auth;
using Directfn.Custody.Api.Requests.User;
using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Authentication.TokenStore;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Passwords;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

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
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuthTokenStore _authTokenStore;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILegacyPasswordService _legacyPasswordService;
        public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, ITokenFingerprintService tokenFingerprintService, IRefreshTokenService refreshTokenService, IAuthTokenStore authTokenStore, ICurrentUserService currentUserService, ILegacyPasswordService legacyPasswordService, IOptions<AuthOptions> authOptions)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _tokenFingerprintService = tokenFingerprintService;
            _refreshTokenService = refreshTokenService;
            _authTokenStore = authTokenStore;
            _currentUserService = currentUserService;
            _legacyPasswordService = legacyPasswordService;
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
                string sessionId = Guid.NewGuid().ToString("N");

                var tokenRequest = new JwtTokenRequest
                {
                    UserId = user.Um02Id.ToString(),
                    UserName = user.Um02LoginId,
                    SessionId = Guid.NewGuid().ToString("N"),
                    FingerprintHash = fingerprint.FingerprintHash,
                    Email = user.Um02Email,
                    Roles = ["CUSTODY_ADMIN"]
                };

                TokenResult token = _jwtTokenService.GenerateAccessToken(tokenRequest);

                string refreshToken = _refreshTokenService.GenerateRefreshToken(tokenRequest);

                string refreshTokenHash = RefreshTokenHasher.Hash(refreshToken);

                string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                string? userAgent = Request.Headers.UserAgent.ToString();

                var session = new AuthSessionRecord
                {
                    SessionId = sessionId,
                    UserId = user.Um02Id.ToString(),
                    UserName = user.Um02LoginId,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                var refreshTokenRecord = new RefreshTokenRecord
                {
                    RefreshTokenId = Guid.NewGuid().ToString("N"),
                    SessionId = sessionId,
                    UserId = user.Um02Id.ToString(),
                    TokenHash = refreshTokenHash,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours),
                    CreatedByIp = ipAddress,
                    UserAgent = userAgent
                };

                await _authTokenStore.CreateSessionAsync(session, cancellationToken);

                await _authTokenStore.StoreRefreshTokenAsync(refreshTokenRecord, cancellationToken);

                SetRefreshTokenCookie(refreshToken);

                token.FirstLogin = user.Um02FirstLogin == 1 ? true : false;

                return Success(token);
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue(_authOptions.RefreshTokenCookieName, out string? refreshToken))
            {
                return Unauthorized(new { Success = false, Message = "Refresh token is missing." });
            }

            RefreshTokenPayload? payload = _refreshTokenService.ValidateRefreshToken(refreshToken);

            if (payload is null)
            {
                return Unauthorized(new { Success = false, Message = "Refresh token is invalid or expired." });
            }

            string currentRefreshTokenHash = RefreshTokenHasher.Hash(refreshToken);

            RefreshTokenValidationResult validationResult = await _authTokenStore.ValidateRefreshTokenAsync(currentRefreshTokenHash, cancellationToken);

            if (!validationResult.IsValid || validationResult.Session is null)
            {
                return Unauthorized(new { Success = false, Message = $"Refresh token rejected. Reason: {validationResult.Status}" });
            }

            TokenFingerprintResult fingerprint = _tokenFingerprintService.Generate();

            SetFingerprintCookie(fingerprint.Fingerprint);

            var tokenRequest = new JwtTokenRequest
            {
                UserId = payload.UserId,
                UserName = payload.UserName,
                SessionId = payload.SessionId,
                FingerprintHash = fingerprint.FingerprintHash,
                Email = payload.Email,
                Roles = payload.Roles
            };

            TokenResult token = _jwtTokenService.GenerateAccessToken(tokenRequest);

            string newRefreshToken = _refreshTokenService.GenerateRefreshToken(tokenRequest);

            string newRefreshTokenHash = RefreshTokenHasher.Hash(newRefreshToken);

            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            string? userAgent = Request.Headers.UserAgent.ToString();

            var newRefreshTokenRecord = new RefreshTokenRecord
            {
                RefreshTokenId = Guid.NewGuid().ToString("N"),
                SessionId = validationResult.Session.SessionId,
                UserId = validationResult.Session.UserId,
                TokenHash = newRefreshTokenHash,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                ExpiresAtUtc = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours),
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            await _authTokenStore.RotateRefreshTokenAsync(currentRefreshTokenHash, newRefreshTokenRecord, cancellationToken);

            SetRefreshTokenCookie(newRefreshToken);

            return Success(token);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (Request.Cookies.TryGetValue(_authOptions.RefreshTokenCookieName, out string? refreshToken))
            {
                RefreshTokenPayload? payload = _refreshTokenService.ValidateRefreshToken(refreshToken);

                if (payload is not null && !string.IsNullOrWhiteSpace(payload.SessionId))
                {
                    await _authTokenStore.RevokeSessionAsync(payload.SessionId, cancellationToken);
                }
            }

            DeleteAuthCookies();

            return Success(new
            {
                LoggedOut = true
            });
        }

        [Authorize]
        [HttpPost("change-first-login-password")]
        public async Task<IActionResult> ChangeFirstLoginPassword([FromBody] ChangeFirstLoginPasswordRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
            {
                return Unauthorized(new { Success = false, Message = "User is not authenticated." });
            }

            long userId = Convert.ToInt64(_currentUserService.UserId);

            string encryptedPassword = _legacyPasswordService.EncryptLegacyPassword(request.NewPassword);

            await _userRepository.ChangeFirstLoginPasswordAsync(userId, encryptedPassword, cancellationToken);

            return Success(new
            {
                PasswordChanged = true,
                Message = "Password changed successfully."
            });
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
        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append(_authOptions.RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = _authOptions.UseSecureCookies,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(_authOptions.RefreshTokenHours)
            });
        }
        private void DeleteAuthCookies()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = _authOptions.UseSecureCookies,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };

            Response.Cookies.Delete(_authOptions.RefreshTokenCookieName, cookieOptions);
            Response.Cookies.Delete(_authOptions.FingerprintCookieName, cookieOptions);
        }
    }
}
