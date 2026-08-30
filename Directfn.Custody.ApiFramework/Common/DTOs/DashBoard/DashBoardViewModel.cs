using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.DashBoard
{
    public class MarketTrades
    {
        public int MT548_SUMMARIZED_COUNT { get; set; }
        public int MT548_MESSAGE_COUNT { get; set; }
        public int MT548_RECONCILED_COUNT { get; set; }
        public int MT548_RECONCILED_SUMM_COUNT { get; set; }
        public int MT548_UNRECONCILED_COUNT { get; set; }
        public int MT548_UNRECONCILED_SUMM_COUNT { get; set; }
        public int CLB_ALLOCATED_COUNT { get; set; }
        public int CLB_ALLOCATED_SUMM_COUNT { get; set; }
        public int CLB_UNALLOCATED_COUNT { get; set; }
        public int CLB_UNALLOCATED_SUMM_COUNT { get; set; }

        public int RELEASED_COUNT { get; set; }
        public int HOLD_COUNT { get; set; }
        public int PENDING_COUNT { get; set; }

    }


    public class Mt530PendingTradesDetail
    {
        public int RELEASED_COUNT { get; set; }
        public int RELEASED_COUNT_SUMM { get; set; }
        public int RELEASED_ACCPT_COUNT { get; set; }
        public int RELEASED_ACCPT_COUNT_SUMM { get; set; }
        public int RELEASED_FAIL_COUNT { get; set; }
        public int RELEASED_FAIL_COUNT_SUMM { get; set; }
        public int PENDING_COUNT { get; set; }
        public int PENDING_COUNT_SUMM { get; set; }
        public int ONHOLD_COUNT { get; set; }
        public int ONHOLD_COUNT_SUMM { get; set; }

        public int ONHOLD_ACCPT_COUNT { get; set; }
        public int ONHOLD_ACCPT_COUNT_SUMM { get; set; }

        public int ONHOLD_FAIL_COUNT { get; set; }
        public int ONHOLD_FAIL_COUNT_SUMM { get; set; }

    }


    public class NetObligations
    {
        public int TOTAL_PURCHASES { get; set; }
        public int ACCEPTED_PURCHASES { get; set; }
        public int TOTAL_SALE { get; set; }
        public int ACCEPTED_SALE { get; set; }
    }


    public class SettlementDetails
    {
        public int MT548_TOTAL_STLMNTS { get; set; }
        public int MT548_TOTAL_STLMNTS_SUMM { get; set; }
        public int MT548_PREV_STLMNTS { get; set; }
        public int MT548_PREV_STLMNTS_SUMM { get; set; }
        public int BUY_RECEIVED { get; set; }
        public int BUY_RECEIVED_SUMM { get; set; }
        public int SELL_RECEIVED { get; set; }
        public int SELL_RECEIVED_SUMM { get; set; }
        public int PARTIAL { get; set; }
        public int PARTIAL_SUMM { get; set; }
        public int MT548_PENDING { get; set; }
        public int MT548_PENDING_SUMM { get; set; }
    }

    public class HeadRoomCalculations
    {
        public int ACTUAL_PURCHASES { get; set; }
        public int ACTUAL_SALES { get; set; }

        public int ACCEPTED_PURCHASES { get; set; }

        public int ACCEPTED_SALES { get; set; }

        public int SETTLEMENT_CAP { get; set; }

        public int LIMIT_UPDATE_EDAA { get; set; }

        public int CAPVALUE_EDAA { get; set; }

    }

    public class OtherSettlementInstructionsDetails
    {
        public string Indicators { get; set; }
        public int Counts { get; set; }
        public int SummarizedCount { get; set; }
    }
}
