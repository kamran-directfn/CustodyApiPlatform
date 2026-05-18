using Directfn.Custody.ApiFramework.Authentication;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;
using Directfn.Custody.ApiFramework.Sessions;

namespace Directfn.Custody.ApiFramework.Middleware;

public sealed class FingerprintValidationMiddleware
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly AuthOptions _authOptions;
    private readonly ITokenFingerprintService _tokenFingerprintService;

    public FingerprintValidationMiddleware(
    RequestDelegate next,
    IOptions<AuthOptions> authOptions,
    ITokenFingerprintService tokenFingerprintService)
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

        var fingerprintHashClaim = context.User.FindFirstValue("fp_hash");

        if (string.IsNullOrWhiteSpace(fingerprintHashClaim))
        {
            await WriteUnauthorizedAsync(
                context,
                "FINGERPRINT_CLAIM_MISSING",
                "Token fingerprint is missing.");

            return;
        }

        if (!context.Request.Cookies.TryGetValue(
                _authOptions.FingerprintCookieName,
                out var fingerprintCookieValue) ||
            string.IsNullOrWhiteSpace(fingerprintCookieValue))
        {
            await WriteUnauthorizedAsync(
                context,
                "FINGERPRINT_COOKIE_MISSING",
                "Token fingerprint cookie is missing.");

            return;
        }

        var computedFingerprintHash =
            _tokenFingerprintService.Hash(fingerprintCookieValue);

        if (!string.Equals(
                computedFingerprintHash,
                fingerprintHashClaim,
                StringComparison.Ordinal))
        {
            await WriteUnauthorizedAsync(
                context,
                "FINGERPRINT_MISMATCH",
                "Token fingerprint validation failed.");

            return;
        }

        // Place session validation here
        var userId =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub")
            ?? context.User.FindFirstValue("user_id");

        var sessionId =
            context.User.FindFirstValue("sid")
            ?? context.User.FindFirstValue("session_id");

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(sessionId))
        {
            await WriteUnauthorizedAsync(
                context,
                "SESSION_CLAIM_MISSING",
                "Token session information is missing.");

            return;
        }

        var isSessionValid = await authSessionService.IsSessionValidAsync(
    userId,
    sessionId,
    context.RequestAborted);

        if (!isSessionValid)
        {
            await WriteUnauthorizedAsync(
                context,
                "SESSION_INVALID",
                "Session is no longer valid.");

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

        var path = context.Request.Path.Value ?? string.Empty;

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

    private static async Task WriteUnauthorizedAsync(
        HttpContext context,
        string errorCode,
        string message)
    {
        var correlationId =
            context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(
            [
                new ApiError
                {
                    Code = errorCode,
                    Message = message
                }
            ],
            correlationId);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonOptions));
    }
}