using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.FOP
{
    public class FOPValidate
    {
        public int p_take;
        public int skip;

        public int PRS50_ID { get; set; }
        public string PRS50_TRADE_TYPE { get; set; }
        public string PRS50_TRADE_DATE { get; set; }
        public string PRS50_SETT_DATE { get; set; }
        public string PRS50_ISIN { get; set; }
        public string PRS50_BROKER { get; set; }
        public long PRS50_QUANTITY { get; set; }
        public string PRS50_UNIQUE_REFERENCE { get; set; }
        public string PRS50_SENDER_CUSTODIAN { get; set; }
        public string PRS50_ACC_SENDER_CUSTODIAN { get; set; }
        public string PRS50_RECEIVER_CUSTODIAN { get; set; }
        public string PRS50_ACC_RECEIVER_CUSTODIAN { get; set; }
        public int PRS50_IS_SENT { get; set; }
        public int PRS50_IS_ACKNOWLEDGE { get; set; }
        public int PRS50_STATUS { get; set; }
        public int PRS50_CREATED_BY { get; set; }
        public string PRS50_PLACE_OF_STLMNT { get; set; }
        public DateTime PRS50_CREATED_DATE { get; set; }
        public int PRS50_MODIFIED_BY { get; set; }
        public DateTime PRS50_MODIFIED_DATE { get; set; }
        public int PRS50_RF48_ID { get; set; }
        public int PRS50_RF42_ID { get; set; }
        public string PRS50_Req_Msg { get; set; }
        public string PRS50_Message_Id { get; set; }
        public int IS_VALID { get; set; }
        public int isShownAtGrid { get; set; }
        public string PRS50_Validation_Status { get; set; }
        public string msg_ref { get; set; }
        public string PRS50_TRANSFER_TYPE { get; set; }
        public string PRS50_ACK_STATUS { get; set; }
        public string PRS50_HOLD_REQ_MSG { get; set; }
        public string PRS50_RELEASE_REQ_MSG { get; set; }
        public string PRS50_CANCEL_REQ_MSG { get; set; }
        public string PRS50_HOLD_STATUS { get; set; }
        public string PRS50_RELEASE_STATUS { get; set; }
        public string PRS50_CANCEL_STATUS { get; set; }
        public string PRS50_SETT_STATUS { get; set; }
        public string PRS50_REASON { get; set; }
        public string PRS50_QUANTITY_TYPE { get; set; }
        public string GuId { get; set; }
        public int BatchId { get; set; }
        public int totalCount { get; set; }
        public string sorting { get; set; }
    }

    public class FOP_Child_Data
    {
        public string prs50_trade_type { get; set; }
        public string quantity { get; set; }
        public string tradedate { get; set; }
        public string prs38_sett_date { get; set; }
        public string PROCESSING_STATUS { get; set; }
        public string SETTLEMENT_STATUS { get; set; }
        public string MATCH_STATUS { get; set; }
        public string RAW_MESSAGE { get; set; }
        public string msg_ref { get; set; }

        public string REASON { get; set; }
    }
}
