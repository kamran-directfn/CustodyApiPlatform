using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.MarketSector;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.MarketSector;
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
    [Route("api/v{version:apiVersion}/market-sector")]
    [ApiController]
    public class MarketSectorController : CustodyControllerBase
    {
        private readonly IMarketSectorRepository _marketSectorRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public MarketSectorController(IMarketSectorRepository marketSectorRepository, ICustodyUserContext custodyUserContext)
        {
            _marketSectorRepository = marketSectorRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_SECTORS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<MarketSectorViewModel> data = await _marketSectorRepository.GetAllMarketSectorsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SECTOR_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int sectorId, CancellationToken cancellationToken)
        {
            MarketSectorViewModel data = await _marketSectorRepository.GetSectorById(sectorId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_SECTOR")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<MarketSectorViewModel> data = await _marketSectorRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_SECTOR")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<MarketSectorViewModel> data = await _marketSectorRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_SECTOR")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _marketSectorRepository.DeleteSector(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_SECTOR")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(MarketSectorReqModel request, CancellationToken cancellationToken)
        {
            request.RF40_CREATED_BY = (int)_custodyUserContext.UserId;
            MarketSectorReqModel data = await _marketSectorRepository.AddMarketSector(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_SECTOR")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(MarketSectorReqModel request, CancellationToken cancellationToken)
        {
            request.RF40_MODIFIED_BY = (int)_custodyUserContext.UserId;
            MarketSectorReqModel data = await _marketSectorRepository.UpdateMarketSector(request, cancellationToken);

            return Success(data);
        }
    }
}
