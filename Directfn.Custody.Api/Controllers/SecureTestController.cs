using Asp.Versioning;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
    [Authorize]
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/secure-test")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public sealed class SecureTestController : CustodyControllerBase
    {
        
        private readonly ICustodyUserContext _custodyUserContext;

        public SecureTestController(ICustodyUserContext userContext)
        {
             
            _custodyUserContext = userContext;
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var data = new
            {
                _custodyUserContext.IsAuthenticated,
                _custodyUserContext.UserId,
                _custodyUserContext.UserName,
                _custodyUserContext.SessionId,
                _custodyUserContext.Email,
                Claims = _custodyUserContext.Claims.Select(x => new { x.Type, x.Value })
            };

            return Success(data);
        }

        [HttpGet("context")]
        public IActionResult Context()
        {
            return Success(new
            {
                _custodyUserContext.IsAuthenticated,
                _custodyUserContext.UserId,
                _custodyUserContext.UserName,
                _custodyUserContext.Email,
                _custodyUserContext.SessionId,
                _custodyUserContext.MemberCode,
                _custodyUserContext.MemberCodeId,
                _custodyUserContext.IpAddress,
                _custodyUserContext.UserAgent,
                _custodyUserContext.CorrelationId
            });
        }
    }
}
