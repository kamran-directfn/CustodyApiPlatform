using Asp.Versioning;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.Api.Requests.User;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user-db-test")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public sealed class UserDbTestController : CustodyControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserDbTestController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> GetLoginUser([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            LoginUserRecord? user = await _userRepository.GetUserForLoginAsync(request.LoginId, request.Rf48Code, cancellationToken);

            if (user is null)
            {
                return Success(new { Found = false, Message = "User was not found." });
            }

            return Success(new
            {
                Found = true,
                User = new
                {
                    user.Um02Id,
                    user.Um02Name,
                    user.Um02LoginId,
                    HasPassword = !string.IsNullOrWhiteSpace(user.Um02Password),
                    user.Um02Email,
                    user.Um02Status,
                    user.Um02LockAccount,
                    user.Um02IsLdap,
                    user.Um02AttemptNo,
                    user.Um03Name,
                    user.Um09Um14Id,
                    user.Um14GroupName
                }
            });
        }
    }
}
