using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Bank;
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
    [Route("api/v{version:apiVersion}/banks")]
    [ApiController]
    public class BanksController : CustodyControllerBase
    {
        private readonly IBankRepository _bankRepository;
        private readonly ICustodyUserContext _custodyUserContext;

        public BanksController(IBankRepository bankRepository, ICustodyUserContext custodyUserContext)
        {
            _bankRepository = bankRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_BANKS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<BanksViewModel> data = await _bankRepository.GetAllBanksAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_BANK_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int bankId, CancellationToken cancellationToken)
        {
            BanksViewModel data = await _bankRepository.GetBankById(bankId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_BANK")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_BANK")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_BANK")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bankRepository.DeleteBank(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_BANK")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(BankReqModel request, CancellationToken cancellationToken)
        {
            request.RF03_MODIFIED_BY = (int)_custodyUserContext.UserId;
            var data = await _bankRepository.SaveBank(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_BANK")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(BankReqModel request, CancellationToken cancellationToken)
        {
            request.RF03_MODIFIED_BY = (int)_custodyUserContext.UserId;
            var data = await _bankRepository.UpdateBank(request, cancellationToken);

            return Success(data);
        }
    }
}
