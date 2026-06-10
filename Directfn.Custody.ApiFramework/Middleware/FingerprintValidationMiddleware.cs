using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Directfn.Custody.ApiFramework.Sessions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Middleware
{
    public sealed class FingerprintValidationMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly AuthOptions _authOptions;

        private readonly RequestDelegate _next;
        private readonly ITokenFingerprintService _tokenFingerprintService;

        public FingerprintValidationMiddleware(RequestDelegate next, IOptions<AuthOptions> authOptions, ITokenFingerprintService tokenFingerprintService)
        {
            _next = next;
            _authOptions = authOptions.Value;
            _tokenFingerprintService = tokenFingerprintService;
        }

        public async Task InvokeAsync(HttpContext context, IAuthSessionService authSessionService)
        {
            if (!ShouldValidateFingerprint(context))
            {
                await _next(context);
                return;
            }

            string? fingerprintHashClaim = context.User.FindFirstValue("fp_hash");

            if (string.IsNullOrWhiteSpace(fingerprintHashClaim))
            {
                await WriteUnauthorizedAsync(context, "FINGERPRINT_CLAIM_MISSING", "Token fingerprint is missing.");

                return;
            }

            if (!context.Request.Cookies.TryGetValue(_authOptions.FingerprintCookieName, out string? fingerprintCookieValue) || string.IsNullOrWhiteSpace(fingerprintCookieValue))
            {
                await WriteUnauthorizedAsync(context, "FINGERPRINT_COOKIE_MISSING", "Token fingerprint cookie is missing.");

                return;
            }

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
                await WriteUnauthorizedAsync(context, "SESSION_CLAIM_MISSING", "Token session information is missing.");

                return;
            }

            bool isSessionValid = await authSessionService.IsSessionValidAsync(userId, sessionId, context.RequestAborted);

            if (!isSessionValid)
            {
                await WriteUnauthorizedAsync(context, "SESSION_INVALID", "Session is no longer valid.");

                return;
            }

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
