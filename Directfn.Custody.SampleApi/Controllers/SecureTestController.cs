using Asp.Versioning;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.SampleApi.Controllers
{
    [Authorize]
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/secure-test")]
    public sealed class SecureTestController : CustodyControllerBase
    {
        private readonly ICurrentUserService _currentUserService;

        public SecureTestController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var data = new
            {
                _currentUserService.IsAuthenticated,
                _currentUserService.UserId,
                _currentUserService.UserName,
                _currentUserService.SessionId,
                _currentUserService.Email,
                Claims = _currentUserService.Claims.Select(x => new { x.Type, x.Value })
            };

            return Success(data);
        }
    }
}