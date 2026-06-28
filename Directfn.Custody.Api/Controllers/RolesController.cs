using Asp.Versioning;
using Directfn.Custody.ApiFramework.Approvals;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Roles;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
 
    [Authorize]
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    public class RolesController : CustodyControllerBase
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly ICurrentUserService _currentUserService;
        public RolesController(IRolesRepository rolesRepository, ICurrentUserService currentUserService)
        {
            _rolesRepository = rolesRepository;
            _currentUserService = currentUserService;
        }

        [AuditAction("GET_ROLES")]
        [HttpGet("get-roles")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<RoleViewModel> data = await _rolesRepository.GetAllRoles(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_ROLES-BY-ID")]
        [HttpGet("get-roles-by-id")]
        public async Task<IActionResult> GetById(int roleId, CancellationToken cancellationToken)
        {
            List<Group> entitlments = new List<Group>();

            RoleViewModel roledata = await _rolesRepository.GetRoleById(roleId,cancellationToken);

            if (roledata != null) {
                entitlments = await _rolesRepository.GetEntitlmentsOfRole(roleId, cancellationToken);

            }

            return Success(new { role = roledata, entitlment = entitlments });
        }

        [AuditAction("POST_ROLE")]
        [HttpPost("post-role")]
        [RequireOperationApprovalCheck("roles", "Um03_Id")]
        public async Task<IActionResult> Post([FromBody] int um02_id, int isPosted, CancellationToken cancellationToken)
        {
            int user_id = 1;
            var data = await _rolesRepository.UpdatePostStatus(um02_id, isPosted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("UNPOST_ROLE")]
        [HttpPost("unpost-role")]
        public async Task<IActionResult> UnPost([FromBody] int um02_id, int isPosted, CancellationToken cancellationToken)
        {
            int user_id = 1;
            var data = await _rolesRepository.UpdatePostStatus(um02_id, isPosted, user_id, cancellationToken);

            return Success(data);
        }
    }
}
