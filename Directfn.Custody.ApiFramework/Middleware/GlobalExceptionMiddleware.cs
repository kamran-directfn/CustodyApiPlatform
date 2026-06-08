using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Exceptions;
using Directfn.Custody.ApiFramework.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException exception)
            {
                await HandleAppExceptionAsync(context, exception);
            }
            catch (Exception exception)
            {
                await HandleUnexpectedExceptionAsync(context, exception);
            }
        }

        private static async Task HandleAppExceptionAsync(HttpContext context, AppException exception)
        {
            string? correlationId = GetCorrelationId(context);

            context.Response.StatusCode = exception.StatusCode;
            context.Response.ContentType = "application/json";

            ApiResponse<object> response = ApiResponse<object>.Fail([
                new ApiError { Code = exception.ErrorCode, Message = exception.Message }
            ], correlationId);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }

        private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception exception)
        {
            string? correlationId = GetCorrelationId(context);

            _logger.LogError(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            ApiResponse<object> response = ApiResponse<object>.Fail([
                new ApiError { Code = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." }
            ], correlationId);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }

        private static string? GetCorrelationId(HttpContext context)
        {
            return context.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
        }
    }
}