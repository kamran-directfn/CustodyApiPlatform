using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.Api.Requests.Roles;
using Directfn.Custody.ApiFramework.Approvals;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.DTOs;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Roles;
using Directfn.Custody.ApiFramework.Security;
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
    [Route("api/v{version:apiVersion}/roles")]
    public class RolesController : CustodyControllerBase
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public RolesController(IRolesRepository rolesRepository, ICustodyUserContext custodyUserContext)
        {
            _rolesRepository = rolesRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("get-CONTROLS")]
        [HttpGet("get-controls")]
        public async Task<IActionResult> Get_Controls(CancellationToken cancellationToken)
        {
            List<UserRoleEntitlements> entitlements = new List<UserRoleEntitlements>();
           // List<RolesEntitlements> lstEntitlements = new List<RolesEntitlements>();
            List<Group> data = new List<Group>();
            entitlements = await _rolesRepository.GetEntitlements(cancellationToken);

           // lstEntitlements = _rolesRepository.GenerateEntitlments(entitlements);

            data = _rolesRepository.MapEntitlements(entitlements);

            return Success(data);
        }

        [AuditAction("GET_ROLES")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<RoleViewModel> data = await _rolesRepository.GetAllRoles(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_ROLES_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int roleId, CancellationToken cancellationToken)
        {
            List<Group> entitlments = new List<Group>();

            RoleViewModel roledata = await _rolesRepository.GetRoleById(roleId,cancellationToken);

            if (roledata != null) 
            {
                entitlments = await _rolesRepository.GetEntitlmentsOfRole(roleId, cancellationToken);
            }

            return Success(new { role = roledata, entitlment = entitlments });
        }

        [AuditAction("APPROVE_ROLE")]
        [HttpPost("approve")]
        //[RequireOperationApprovalCheck("roles", "Um03_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _rolesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_ROLE")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _rolesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("Delete_ROLE")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete (DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _rolesRepository.DeleteRoles(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_ROLE")]
        [HttpPost("save")]
        public async Task<IActionResult> Save(RoleRequest request, CancellationToken cancellationToken)
        {
            request.Role.UM03_CREATED_BY = (int)_custodyUserContext.UserId;

            var data = await _rolesRepository.AddRoles(request.Role, request.Entitlements, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_ROLE")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(RoleRequest request, CancellationToken cancellationToken)
        {
            request.Role.UM03_MODIFIED_BY = (int)_custodyUserContext.UserId;

            var data = await _rolesRepository.UpdateRole(request.Role, request.Entitlements, cancellationToken);

            return Success(data);
        }
    }
}
