using Directfn.Custody.ApiFramework.Repositories.Operations;
using Directfn.Custody.ApiFramework.Responses;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Directfn.Custody.ApiFramework.Approvals;

public sealed class OperationApprovalActionFilter : IAsyncActionFilter
{
    private readonly IOperationApprovalRepository _operationApprovalRepository;
    private readonly ICurrentUserService _currentUserService;

    public OperationApprovalActionFilter(IOperationApprovalRepository operationApprovalRepository, ICurrentUserService currentUserService)
    {
        _operationApprovalRepository = operationApprovalRepository;
        _currentUserService = currentUserService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        RequireOperationApprovalCheckAttribute? attribute = GetAttribute(context);

        if (attribute is null)
        {
            await next();
            return;
        }

        if (string.IsNullOrWhiteSpace(_currentUserService.UserId))
        {
            context.Result = BuildForbiddenResult("User is not authenticated.");
            return;
        }

        string? memberCodeIdValue = context.HttpContext.User.FindFirst("member_code_id")?.Value;

        if (string.IsNullOrWhiteSpace(memberCodeIdValue))
        {
            context.Result = BuildForbiddenResult("Member code ID was not found in token.");
            return;
        }

        string? recordId = GetRecordIdFromActionArguments(context, attribute.RecordIdPropertyName);

        if (string.IsNullOrWhiteSpace(recordId))
        {
            context.Result = BuildForbiddenResult("Record ID was not found in request.");
            return;
        }

        long userId = Convert.ToInt64(_currentUserService.UserId);
        long memberCodeId = Convert.ToInt64(memberCodeIdValue);

        var approvalResult = await _operationApprovalRepository.CheckUserCanPerformOperationAsync(userId, memberCodeId, attribute.ScreenName, recordId, context.HttpContext.RequestAborted);

        if (approvalResult is null)
        {
            context.Result = BuildForbiddenResult("Operation approval check failed.");
            return;
        }

        if (!string.Equals(approvalResult.Status, "valid", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = BuildForbiddenResult(approvalResult.Comments ?? "You cannot perform this operation.");
            return;
        }

        await next();
    }

    private static RequireOperationApprovalCheckAttribute? GetAttribute(ActionExecutingContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
        {
            return null;
        }

        return actionDescriptor.MethodInfo.GetCustomAttributes(typeof(RequireOperationApprovalCheckAttribute), inherit: true).OfType<RequireOperationApprovalCheckAttribute>().FirstOrDefault();
    }

    private static string? GetRecordIdFromActionArguments(ActionExecutingContext context, string propertyName)
    {
        foreach (object? argumentValue in context.ActionArguments.Values)
        {
            if (argumentValue is null)
            {
                continue;
            }

            PropertyInfo? property = argumentValue.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property is null)
            {
                continue;
            }

            object? value = property.GetValue(argumentValue);

            return Convert.ToString(value);
        }

        return null;
    }

    private static IActionResult BuildForbiddenResult(string message)
    {
        var response = ApiResponse<object>.Fail(
        [
            new ApiError
            {
                Code = "OPERATION_NOT_ALLOWED",
                Message = message
            }
        ]);

        return new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}