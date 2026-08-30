using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment
{
    public class FOPAllegmentViewModel
    {
        public int prs62ID { get; set; }
        public string PRS62_MSGREF { get; set; }
        public string PRS62_PLACEOFSETTLMENT { get; set; }
        public string PRS62_ISIN { get; set; }
        public string PRS62_REC_AGNET { get; set; }
        public string PRS62_DEL_AGNET { get; set; }
        public string PRS62_SETT_DATE { get; set; }
        public string PRS62_TRADE_DATE { get; set; }
        public string PRS62_MSGFUNCTION { get; set; }
        public string PRS62_PREPARATION_DATE { get; set; }
        public string PRS62_TYPE_AMOUNT_INSTRUMENT { get; set; }
        public string PRS62_PAYMENT_INDICATOR { get; set; }
        public string PRS62_DELIVERY_INDICATOR { get; set; }
        public string PRS62_SAFEKEEPING_ACC { get; set; }
        public string PRS62_TRANSACTION_TYPE { get; set; }
        public string PRS62_AMOUNT_USE_SETTLEMENT { get; set; }
        public string PRS62_TRANS_SETTL_INDICATOR { get; set; }
        public string PRS62STATUS { get; set; }
        public string PRS62_RAW_MSG { get; set; }
        public string PRS62_REL_REF { get; set; }
        public string PRS62_ICMS_STATUS { get; set; }
        public int PRS62_IS_CANCELLED { get; set; }
        public string PRS62_IS_CANCELLED_Des { get; set; }
        public string PRS62_QUANTITY { get; set; }
        public string PRS62_CANC_MSG { get; set; }
        public int PRS62CREATED_BY { get; set; }
        public DateTime PRS62CREATED_DATE { get; set; }
        public int PRS62MODIFIED_BY { get; set; }
        public DateTime PRS62MODIFIED_DATE { get; set; }
        public string rf02_symbol { get; set; }
        public string PRS62_QUANTITY_TYPE { get; set; }
    }
}
