using System.Diagnostics;
using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Auditing;

public sealed class AuditActionFilter : IAsyncActionFilter
{
    private readonly IAuditWriter _auditWriter;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly AuditOptions _auditOptions;

    public AuditActionFilter(IAuditWriter auditWriter, ICurrentUserService currentUserService, ICorrelationIdAccessor correlationIdAccessor, IOptions<AuditOptions> auditOptions)
    {
        _auditWriter = auditWriter;
        _currentUserService = currentUserService;
        _correlationIdAccessor = correlationIdAccessor;
        _auditOptions = auditOptions.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_auditOptions.Enabled || ShouldSkipAudit(context))
        {
            await next();
            return;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        ActionExecutedContext? executedContext = null;
        Exception? exception = null;

        try
        {
            executedContext = await next();
            exception = executedContext.Exception;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            AuditEvent auditEvent = CreateAuditEvent(context, executedContext, exception, stopwatch.ElapsedMilliseconds);

            await _auditWriter.WriteAsync(auditEvent, context.HttpContext.RequestAborted);
        }
    }

    private AuditEvent CreateAuditEvent(ActionExecutingContext context, ActionExecutedContext? executedContext, Exception? exception, long durationMs)
    {
        ControllerActionDescriptor? actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        int? statusCode = GetStatusCode(context, executedContext, exception);

        return new AuditEvent
        {
            UserId = _currentUserService.UserId,
            UserName = _currentUserService.UserName,
            SessionId = _currentUserService.SessionId,
            CorrelationId = _correlationIdAccessor.CorrelationId,
            HttpMethod = context.HttpContext.Request.Method,
            Path = context.HttpContext.Request.Path.Value,
            ControllerName = actionDescriptor?.ControllerName,
            ActionName = actionDescriptor?.ActionName,
            AuditAction = GetAuditActionName(context),
            StatusCode = statusCode,
            Succeeded = exception is null && statusCode is >= 200 and < 400,
            DurationMs = durationMs,
            IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = context.HttpContext.Request.Headers.UserAgent.ToString(),
            ErrorMessage = exception?.Message
        };
    }

    private static bool ShouldSkipAudit(ActionExecutingContext context)
    {
        ControllerActionDescriptor? actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        if (actionDescriptor is null)
        {
            return false;
        }

        bool skipOnController = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(SkipAuditAttribute), inherit: true).Any();

        bool skipOnAction = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(SkipAuditAttribute), inherit: true).Any();

        return skipOnController || skipOnAction;
    }

    private static string? GetAuditActionName(ActionExecutingContext context)
    {
        ControllerActionDescriptor? actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        if (actionDescriptor is null)
        {
            return null;
        }

        AuditActionAttribute? methodAttribute = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AuditActionAttribute), inherit: true).OfType<AuditActionAttribute>().FirstOrDefault();

        if (methodAttribute is not null)
        {
            return methodAttribute.ActionName;
        }

        AuditActionAttribute? controllerAttribute = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AuditActionAttribute), inherit: true).OfType<AuditActionAttribute>().FirstOrDefault();

        return controllerAttribute?.ActionName;
    }

    private static int? GetStatusCode(ActionExecutingContext context, ActionExecutedContext? executedContext, Exception? exception)
    {
        if (exception is not null)
        {
            return 500;
        }

        if (executedContext?.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
        {
            return objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
        }

        if (executedContext?.Result is Microsoft.AspNetCore.Mvc.StatusCodeResult statusCodeResult)
        {
            return statusCodeResult.StatusCode;
        }

        return context.HttpContext.Response.StatusCode;
    }
}