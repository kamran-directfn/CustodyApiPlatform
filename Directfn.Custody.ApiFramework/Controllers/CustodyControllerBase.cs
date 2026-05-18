using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.ApiFramework.Controllers;

[ApiController]
public abstract class CustodyControllerBase : ControllerBase
{
    protected IActionResult Success<T>(T data)
    {
        var correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        return Ok(ApiResponse<T>.Ok(data, correlationId));
    }

    protected IActionResult CreatedSuccess<T>(
        string actionName,
        object routeValues,
        T data)
    {
        var correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        return CreatedAtAction(
            actionName,
            routeValues,
            ApiResponse<T>.Ok(data, correlationId));
    }
}