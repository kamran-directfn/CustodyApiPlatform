using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.BicCodeConfig;
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
    [Route("api/v{version:apiVersion}/bic-code-config")]
    [ApiController]
    public class BicCodeConfigController : CustodyControllerBase
    {
        private readonly IBicCodeConfigRepository _bicCodeConfigRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public BicCodeConfigController(IBicCodeConfigRepository bicCodeConfigRepository, ICustodyUserContext custodyUserContext)
        {
            _bicCodeConfigRepository = bicCodeConfigRepository;
            _custodyUserContext = custodyUserContext;
        }
       

        [AuditAction("GET_BIC_CODE_CONFIGS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<BicCodeConfigViewModel> data = await _bicCodeConfigRepository.GetAllBicCodeConfigsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_BIC_CODE_CONFIG_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int bicCodeId, CancellationToken cancellationToken)
        {
            BicCodeConfigViewModel data = await _bicCodeConfigRepository.GetBicCodeConfigById(bicCodeId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_BIC_CODE_CONFIG")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<BicCodeConfigViewModel> data = await _bicCodeConfigRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_BIC_CODE_CONFIG")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<BicCodeConfigViewModel> data = await _bicCodeConfigRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_BIC_CODE_CONFIG")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _bicCodeConfigRepository.DeleteBicCodeConfig(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_BIC_CODE_CONFIG")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(BicCodeConfigReqModel request, CancellationToken cancellationToken)
        {
            request.RF84_CREATED_BY = (int)_custodyUserContext.UserId;
            BicCodeConfigReqModel data = await _bicCodeConfigRepository.AddBicCodeConfig(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_BIC_CODE_CONFIG")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(BicCodeConfigReqModel request, CancellationToken cancellationToken)
        {
            request.RF84_MODIFIED_BY = (int)_custodyUserContext.UserId;
            BicCodeConfigReqModel data = await _bicCodeConfigRepository.UpdateBicCodeConfig(request, cancellationToken);

            return Success(data);
        }
    }
}
