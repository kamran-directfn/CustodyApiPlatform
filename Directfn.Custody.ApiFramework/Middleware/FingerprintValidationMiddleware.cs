using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Directfn.Custody.ApiFramework.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Middleware
{
    public sealed class FingerprintValidationMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly AuthOptions _authOptions;
        private readonly ILogger<FingerprintValidationMiddleware> _logger;  

        private readonly RequestDelegate _next;
        private readonly ITokenFingerprintService _tokenFingerprintService;

        public FingerprintValidationMiddleware(RequestDelegate next, IOptions<AuthOptions> authOptions, ITokenFingerprintService tokenFingerprintService, ILogger<FingerprintValidationMiddleware> logger)
        {
            _next = next;
            _authOptions = authOptions.Value;
            _tokenFingerprintService = tokenFingerprintService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthSessionService authSessionService)
        {
            if (!ShouldValidateFingerprint(context))
            {
                _logger.LogDebug("Fingerprint validation skipped for path {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            _logger.LogInformation("Fingerprint validation started for path {Path}", context.Request.Path);

            _logger.LogInformation("Incoming cookies: {Cookies}", string.Join(", ", context.Request.Cookies.Keys));

            string? fingerprintHashClaim = context.User.FindFirstValue("fp_hash");

            if (string.IsNullOrWhiteSpace(fingerprintHashClaim))
            {
                _logger.LogWarning("Fingerprint validation failed. Missing fp_hash claim. Path={Path}", context.Request.Path);
                await WriteUnauthorizedAsync(context, "FINGERPRINT_CLAIM_MISSING", "Token fingerprint is missing.");

                return;
            }

            if (!context.Request.Cookies.TryGetValue(_authOptions.FingerprintCookieName, out string? fingerprintCookieValue) || string.IsNullOrWhiteSpace(fingerprintCookieValue))
            {
                _logger.LogWarning("Fingerprint validation failed. Fingerprint hash mismatch. Path={Path}", context.Request.Path);
                await WriteUnauthorizedAsync(context, "FINGERPRINT_MISMATCH", "Fingerprint validation failed.");
                return;
            }

            _logger.Log(LogLevel.Debug, fingerprintCookieValue);

            string computedFingerprintHash = _tokenFingerprintService.Hash(fingerprintCookieValue);

            if (!string.Equals(computedFingerprintHash, fingerprintHashClaim, StringComparison.Ordinal))
            {
                await WriteUnauthorizedAsync(context, "FINGERPRINT_MISMATCH", "Token fingerprint validation failed.");

                return;
            }

            // Place session validation here
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub") ?? context.User.FindFirstValue("user_id");

            string? sessionId = context.User.FindFirstValue("sid") ?? context.User.FindFirstValue("session_id");

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(sessionId))
            {
                _logger.LogWarning("Fingerprint validation failed. Missing session claims. UserId={UserId}, SessionId={SessionId}, Path={Path}", userId, sessionId, context.Request.Path);
                await WriteUnauthorizedAsync(context, "SESSION_CLAIM_MISSING", "Session claim is missing.");
                return;
            }

            bool isSessionValid = await authSessionService.IsSessionValidAsync(userId, sessionId, context.RequestAborted);

            if (!isSessionValid)
            {
                _logger.LogWarning("Fingerprint validation failed. Session invalid. UserId={UserId}, SessionId={SessionId}, Path={Path}", userId, sessionId, context.Request.Path);
                await WriteUnauthorizedAsync(context, "SESSION_INVALID", "Session is invalid.");
                return;
            }

            _logger.LogInformation("Fingerprint validation succeeded. UserId={UserId}, SessionId={SessionId}, Path={Path}", userId, sessionId, context.Request.Path);

            await _next(context);
        }

        private static bool ShouldValidateFingerprint(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            string path = context.Request.Path.Value ?? string.Empty;

            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        private static async Task WriteUnauthorizedAsync(HttpContext context, string errorCode, string message)
        {
            string? correlationId = context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            ApiResponse<object> response = ApiResponse<object>.Fail([
                new ApiError { Code = errorCode, Message = message }
            ], correlationId);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }
    }
}
