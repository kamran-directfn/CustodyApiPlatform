using Asp.Versioning;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.SampleApi.Controllers
{
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class HealthTestController : CustodyControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var data = new { Status = "Running", Application = "Directfn Custody Sample API", ApiVersion = "v1", Timestamp = DateTime.UtcNow };

            return Success(data);
        }

        [HttpGet("not-found")]
        public IActionResult TestNotFound()
        {
            throw new NotFoundException("ACCOUNT_NOT_FOUND", "Account was not found.");
        }

        [HttpGet("business-error")]
        public IActionResult TestBusinessError()
        {
            throw new BusinessRuleException("SETTLEMENT_ALREADY_APPROVED", "Settlement is already approved.");
        }

        [HttpGet("unexpected-error")]
        public IActionResult TestUnexpectedError()
        {
            throw new InvalidOperationException("Testing unexpected exception.");
        }
    }
}