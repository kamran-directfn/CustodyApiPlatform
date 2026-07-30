using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.SubMarket;
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
    [Route("api/v{version:apiVersion}/sub-market")]
    [ApiController]
    public class SubMarketController : CustodyControllerBase
    {
        private readonly ISubMarketRepository _subMarketRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public SubMarketController(ISubMarketRepository subMarketRepository, ICustodyUserContext custodyUserContext)
        {
            _subMarketRepository = subMarketRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_SUB_MARKETS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<SubMarketViewModel> data = await _subMarketRepository.GetAllSubMarketsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SUB_MARKET_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int marketId, CancellationToken cancellationToken)
        {
            SubMarketViewModel data = await _subMarketRepository.GetSubMarketById(marketId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_SUB_MARKET")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<SubMarketViewModel> data = await _subMarketRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_SUB_MARKET")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<SubMarketViewModel> data = await _subMarketRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_SUB_MARKET")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _subMarketRepository.DeleteSubMarket(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_SUB_MARKET")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(SubMarketReqModel request, CancellationToken cancellationToken)
        {
            request.RF16_CREATED_BY = (int)_custodyUserContext.UserId;
            SubMarketReqModel data = await _subMarketRepository.AddSubMarket(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_SUB_MARKET")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(SubMarketReqModel request, CancellationToken cancellationToken)
        {
            request.RF16_MODIFIED_BY = (int)_custodyUserContext.UserId;
            SubMarketReqModel data = await _subMarketRepository.UpdateSubMarket(request, cancellationToken);

            return Success(data);
        }
    }
}
