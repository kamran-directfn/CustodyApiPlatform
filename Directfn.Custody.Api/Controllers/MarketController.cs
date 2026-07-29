using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Market;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Market;
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
    [Route("api/v{version:apiVersion}/market")]
    [ApiController]

    public class MarketController : CustodyControllerBase
    {
        private readonly IMarketRepository _marketRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public MarketController(IMarketRepository marketRepository, ICustodyUserContext custodyUserContext)
        {
            _marketRepository = marketRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_MARKETS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<MarketViewModel> data = await _marketRepository.GetAllMarketsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_MARKET_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int marketId, CancellationToken cancellationToken)
        {
            MarketViewModel data = await _marketRepository.GetMarketById(marketId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_MARKET")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<MarketViewModel> data = await _marketRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_MARKET")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<MarketViewModel> data = await _marketRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_MARKET")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _marketRepository.DeleteMarket(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_MARKET")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(MarketReqModel request, CancellationToken cancellationToken)
        {
            request.RF01_CREATED_BY = (int)_custodyUserContext.UserId;
            MarketReqModel data = await _marketRepository.AddMarket(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_MARKET")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(MarketReqModel request, CancellationToken cancellationToken)
        {
            request.RF01_MODIFIED_BY = (int)_custodyUserContext.UserId;
            MarketReqModel data = await _marketRepository.UpdateMarket(request, cancellationToken);

            return Success(data);
        }
    }
}
