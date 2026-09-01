using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement
{
    public class FOPSettlementExport
    {
        public string TRADE_TYPE { get; set; }
        public string TRANSFER_TYPE { get; set; }
        public string TRADE_DATE { get; set; }
        public string SETTLEMENT_DATE { get; set; }
        public string SYMBOL { get; set; }
        public string ISIN { get; set; }
        public string QUANTITY { get; set; }
        public string UNIQUE_REFERENCE { get; set; }
        public string SENDER_CUSTODIAN { get; set; }
        public string SENDER_CUSTODIAN_ACCOUNT { get; set; }
        public string RECEIVER_CUSTODIAN { get; set; }
        public string RECEIVER_CUSTODIAN_ACCOUNT { get; set; }
        public string MESSAGE_REF { get; set; }
        public string REQUEST_MESSAGE { get; set; }
        public string HOLD_REQUEST_MESSAGE { get; set; }
        public string CANCEL_REQUEST_MESSAGE { get; set; }
        public string RELEASE_REQUEST_MESSAGE { get; set; }
    }
}
