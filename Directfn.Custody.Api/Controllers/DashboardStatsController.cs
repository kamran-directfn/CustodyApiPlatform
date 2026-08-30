using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.DashBoard;
using Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.DashBoard;
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
    [Route("api/v{version:apiVersion}/dashboard")]
    [ApiController]
    public class DashboardStatsController : CustodyControllerBase
    {
        private readonly IDashBoardRepository _dashBoardRepository;
        private readonly ICustodyUserContext _custodyUserContext;

        public DashboardStatsController(IDashBoardRepository dashBoardRepository, ICustodyUserContext custodyUserContext)
        {
            _dashBoardRepository = dashBoardRepository;
            _custodyUserContext = custodyUserContext;
        }

        //[AuditAction("GET_HEAD_ROOM_CONFIGS")]
        [HttpGet("get-trade-messages-count-summary")]
        public async Task<IActionResult> GetTradeMessagesCountSummary(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                MarketTrades data = await _dashBoardRepository.GetTradeMessagesCountSummary(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-mt530-pending-trade-detail")]
        public async Task<IActionResult> GetMt530PendingTradeDetail(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                Mt530PendingTradesDetail data = await _dashBoardRepository.GetMt530PendingTradeDetail(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-net-obligations")]
        public async Task<IActionResult> GetNetObligations(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                NetObligations data = await _dashBoardRepository.GetNetObligations(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-settlement-details")]
        public async Task<IActionResult> GetSettlementDetails(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                SettlementDetails data = await _dashBoardRepository.GetSettlementDetails(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-confirmation-details")]
        public async Task<IActionResult> GetConfirmationDetails(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                SettlementDetails data = await _dashBoardRepository.GetSettlementDetails(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-trade-forcast-details")]
        public async Task<IActionResult> GetTradeForcastDetails(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                HeadRoomCalculations data = await _dashBoardRepository.GetForcastDetails(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }

        [HttpGet("get-other-settlement-instruction-details")]
        public async Task<IActionResult> GetOtherSettlementInstructionDetails(DateTime settlementDate, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                int rf48_id = (int)_custodyUserContext.MemberCodeId;
                int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

                OtherSettlementInstructionsDetails data = await _dashBoardRepository.GetOtherSettlementInstructionDetails(settlementDate, rf48_id, portfolioId, cancellationToken);

                return Success(data);
            });
        }
    }
}
