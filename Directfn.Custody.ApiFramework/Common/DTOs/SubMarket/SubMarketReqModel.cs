using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.SubMarket
{
    public class SubMarketReqModel
    {
        public int RF16_SUB_MARKET_ID { get; set; }
        public string RF16_MARKET_CODE { get; set; }
        public string RF16_DESCRIPTION { get; set; }
        public string RF16_DESCRIPTION_SEC { get; set; }
        public string RF16_CURRENCY { get; set; }
        public string RF16_START_TIME { get; set; }
        public string RF16_END_TIME { get; set; }
        public int RF16_RF01_ID { get; set; }
        public int RF16_TPLUSN_SELL { get; set; }
        public int RF16_TPLUSN_BUY { get; set; }
        public int RF16_MINIMUM_DISCLOSED_QTY { get; set; }
        public string RF16_REUTER_CODE { get; set; }
        public int RF16_CREATED_BY { get; set; }
        public int RF16_MODIFIED_BY { get; set; }
        public string? ERROR_MESSAGE { get; set; }
    }
}
