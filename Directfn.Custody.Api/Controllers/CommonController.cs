using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/common")]
    public sealed class CommonController : CustodyControllerBase
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IUserRepository _userRepository;
        public CommonController(ICommonRepository commonRepository, IUserRepository userRepository)
        {
            _commonRepository = commonRepository;
            _userRepository = userRepository;
        }

        [AuditAction("GET_ROLES")]
        [HttpGet("get_roles")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetRoles(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_MEMBER_CODES")]
        [HttpGet("get-member-codes")]
        public async Task<IActionResult> GetMemberCode(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetMemberCode(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SUPERVISORS")]
        [HttpGet("get-supervisors")]
        public async Task<IActionResult> getSupervisorDropdown(CancellationToken cancellationToken)
        {
            List<UserViewModel> user = await _userRepository.GetAllUserAsync(cancellationToken);

            List<DropDowns> data = user.AsEnumerable().Select(item => new DropDowns() { Id = item.UM02_ID.ToString(), text = item.UM02_NAME }).ToList();
            
            return Success(data);
        }
    }
}
