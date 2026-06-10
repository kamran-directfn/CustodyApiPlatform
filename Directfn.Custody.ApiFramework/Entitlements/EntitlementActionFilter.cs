using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Directfn.Custody.ApiFramework.Entitlements
{
    public sealed class EntitlementActionFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEntitlementService _entitlementService;

        public EntitlementActionFilter(ICurrentUserService currentUserService, IEntitlementService entitlementService)
        {
            _currentUserService = currentUserService;
            _entitlementService = entitlementService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (ShouldSkipEntitlementCheck(context))
            {
                await next();
                return;
            }

            if (!_currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(_currentUserService.UserId))
            {
                context.Result = new ObjectResult(ApiResponse<object>.Fail([
                    new ApiError { Code = "UNAUTHORIZED", Message = "Authentication is required." }
                ], GetCorrelationId(context))) { StatusCode = StatusCodes.Status401Unauthorized };

                return;
            }

            string? controllerName = GetControllerName(context);
            string? actionName = GetActionName(context);

            if (string.IsNullOrWhiteSpace(controllerName) || string.IsNullOrWhiteSpace(actionName))
            {
                context.Result = Forbidden("ACCESS_DENIED", "You do not have permission to perform this action.", true, context);

                return;
            }

            bool hasAccess = await _entitlementService.HasAccessAsync(_currentUserService.UserId, controllerName, actionName, context.HttpContext.RequestAborted);

            if (!hasAccess)
            {
                bool show = true;
                string message = "You do not have permission to perform this action.";

                if (string.Equals(actionName, "Get", StringComparison.OrdinalIgnoreCase))
                {
                    show = false;
                    message = $"You do not have permission to view {controllerName} List.";
                }

                context.Result = Forbidden("ACCESS_DENIED", message, show, context);

                return;
            }

            await next();
        }

        private static bool ShouldSkipEntitlementCheck(ActionExecutingContext context)
        {
            Endpoint? endpoint = context.HttpContext.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<SkipEntitlementAttribute>() is not null)
            {
                return true;
            }

            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
            {
                return true;
            }

            return false;
        }

        private static string? GetControllerName(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                return descriptor.ControllerName;
            }

            return context.RouteData.Values["controller"]?.ToString();
        }

        private static string? GetActionName(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                return descriptor.ActionName;
            }

            return context.RouteData.Values["action"]?.ToString();
        }

        private static ObjectResult Forbidden(string code, string message, bool show, ActionExecutingContext context)
        {
            context.HttpContext.Response.Headers["Status"] = "Forbidden";
            context.HttpContext.Response.Headers["Show"] = show.ToString();
            context.HttpContext.Response.Headers["Text"] = message;

            return new ObjectResult(ApiResponse<object>.Fail([
                new ApiError { Code = code, Message = message }
            ], GetCorrelationId(context))) { StatusCode = StatusCodes.Status403Forbidden };
        }

        private static string? GetCorrelationId(ActionExecutingContext context)
        {
            return context.HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
        }
    }
}
