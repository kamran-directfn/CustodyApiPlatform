using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Approvals;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public sealed class UserController : CustodyControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public UserController(IUserRepository userRepository, ICustodyUserContext custodyUserContext)
        {
            _userRepository = userRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_USER")]
        [HttpGet("get-user")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<UserViewModel> data = await _userRepository.GetAllUserAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_USER_BY_ID")]
        [HttpGet("get-user-by-id")]
        public async Task<IActionResult> GetById(int UserId, CancellationToken cancellationToken)
        {
            UserViewModel data = await _userRepository.GetUserByIDAsync(UserId, cancellationToken);

            return Success(data);
        }

        [AuditAction("VERIFY_USER_NAME")]
        [HttpGet("verify-username")]
        public async Task<IActionResult> VerifyUserName(string userName, CancellationToken cancellationToken)
        {
            var data = await _userRepository.VerifyUserNameAsync(userName, cancellationToken);
            
            return Success(data);
        }

        [AuditAction("SAVE_USER")]
        [HttpPost("save")]
        public async Task<IActionResult> Add([FromBody] UserRequestModel user, CancellationToken cancellationToken)
        {
            int userId = (int)_custodyUserContext.UserId;
            string userName = _custodyUserContext.UserName.ToString();

            user.UM02_ID = await _userRepository.SaveUserAsync(user, cancellationToken);
            
            return Success(user);
        }

        [AuditAction("UPDATE_USER")]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserRequestModel user, CancellationToken cancellationToken)
        {
            int userId = (int)_custodyUserContext.UserId;
            await _userRepository.UpdateUser(user, cancellationToken);

            return Success(user);
        }

        [AuditAction("APPROVE_USER")]
        [HttpPost("approve")]
       // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _userRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_USER")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _userRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_USER")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _userRepository.Delete(request.id, user_id, cancellationToken);
           
            return Success(data);
        }
    }
}
