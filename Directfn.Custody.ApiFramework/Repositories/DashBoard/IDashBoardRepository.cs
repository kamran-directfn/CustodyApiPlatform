using Directfn.Custody.ApiFramework.Common.DTOs.DashBoard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.DashBoard
{
    public interface IDashBoardRepository
    {
        Task<MarketTrades> GetTradeMessagesCountSummary(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
        Task<Mt530PendingTradesDetail> GetMt530PendingTradeDetail(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
        Task<NetObligations> GetNetObligations(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
        Task<SettlementDetails> GetSettlementDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
        Task<HeadRoomCalculations> GetForcastDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
        Task<OtherSettlementInstructionsDetails> GetOtherSettlementInstructionDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken);
    }
}
