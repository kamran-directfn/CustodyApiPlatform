using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Market
{
    public class MarketReqModel
    {
        public int RF01_MARKET_ID { get; set; }
        public string RF01_CODE { get; set; }
        public string RF01_DESCRIPTION { get; set; }
        public string RF01_DESCRIPTION_SEC { get; set; }
        public string RF01_CURRENCY {  get; set; }
        public string RF01_START_TIME { get; set; }
        public string RF01_END_TIME { get; set; }
        public string[] Rf01_Weekend_Arr { get; set; }
        public string? RF01_WEEKEND { get; set; }
        public int RF01_CREATED_BY { get; set; }
        public int RF01_MODIFIED_BY { get; set; }
    }
}
