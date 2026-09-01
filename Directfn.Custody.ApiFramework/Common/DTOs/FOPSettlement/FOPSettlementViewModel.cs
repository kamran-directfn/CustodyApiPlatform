using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement
{
    public class FOPSettlementViewModel
    {
        public int QUANTITY { get; set; }
        public string MESSAGE_REF { get; set; }
        public string UNIQUE_REFERENCE { get; set; }
        public string RECEIVER_CUSTODIAN { get; set; }
        public string ACC_RECEIVER_CUSTODIAN { get; set; }
        public string SENDER_CUSTODIAN { get; set; }
        public string ACC_SENDER_CUSTODIAN { get; set; }
        public string TRADE_TYPE { get; set; }
        public string TRADE_DATE { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string ISIN { get; set; }
        public string TRANSFER_TYPE { get; set; }
        public string rf02_symbol { get; set; }
        public string prs50_req_msg { get; set; }
        public string prs50_hold_req_msg { get; set; }
        public string prs50_release_req_msg { get; set; }
        public string prs50_cancel_req_msg { get; set; }
        public int totalCount { get; set; }

    }

    public class FOPSettlementFilter
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? sorting { get; set; }
        public DateTime? SettlementDate { get; set; }
        public DateTime? TradeDate { get; set; }
        public string? UniqueReference { get; set; }
        public int PortfolioGroupId { get; set; }
        public int Rf48Id { get; set; }
    }
}
