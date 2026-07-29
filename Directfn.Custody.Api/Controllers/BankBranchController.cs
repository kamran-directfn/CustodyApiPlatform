using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.BankBranch;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.BankBranch;
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
    [Route("api/v{version:apiVersion}/bankbranch")]
    [ApiController]
    public class BankBranchController : CustodyControllerBase
    {
        private readonly ICustodyUserContext _custodyUserContext;
        private readonly IBankBranchRepository _bankBranchRepository;

        public BankBranchController(ICustodyUserContext custodyUserContext, IBankBranchRepository bankBranchRepository)
        {
            _custodyUserContext = custodyUserContext;
            _bankBranchRepository = bankBranchRepository;
        }

        [AuditAction("GET_BANK_BRANCH_BRANCHES")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<BankBranchViewModel> data = await _bankBranchRepository.GetAllBranchesAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_BANK_BRANCH_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int branchId, CancellationToken cancellationToken)
        {
            BankBranchViewModel data = await _bankBranchRepository.GetBranchById(branchId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_BANK_BRANCH")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankBranchRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_BANK_BRANCH")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankBranchRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_BANK_BRANCH")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankBranchRepository.DeleteBank(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_BANK_BRANCH")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(BankBranchReqModel request, CancellationToken cancellationToken)
        {
            request.RF04_MODIFIED_BY = (int)_custodyUserContext.UserId;
            var data = await _bankBranchRepository.SaveBank(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_BANK_BRANCH")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(BankBranchReqModel request, CancellationToken cancellationToken)
        {
            request.RF04_MODIFIED_BY = (int)_custodyUserContext.UserId;
            var data = await _bankBranchRepository.UpdateBank(request, cancellationToken);

            return Success(data);
        }
    }
}