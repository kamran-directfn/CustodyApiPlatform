using Directfn.Custody.ApiFramework.Correlation;
using Directfn.Custody.ApiFramework.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.ApiFramework.Controllers
{
    [ApiController]
    public abstract class CustodyControllerBase : ControllerBase
    {
        protected IActionResult Success<T>(T data)
        {
            string? correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

            return Ok(ApiResponse<T>.Ok(data, correlationId));
        }
        protected IActionResult Success<T>(T data,IReadOnlyDictionary<string, object> metadata)
        {
            string? correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

            return Ok(ApiResponse<T>.Ok(data, correlationId, metadata));
        }

        protected IActionResult CreatedSuccess<T>(string actionName, object routeValues, T data)
        {
            string? correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

            return CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, correlationId));
        }

        //protected IActionResult ApiError<T>(T data, int statusCode)
        //{
        //    string? correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();

        //    var response = new ApiResponse<T>
        //    {
        //        Success = false,
        //        Data = data,
        //        CorrelationId = correlationId
        //    };

        //    return StatusCode(statusCode, response);
        //}
    }
}
