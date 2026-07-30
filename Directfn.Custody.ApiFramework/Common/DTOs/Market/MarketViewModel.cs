using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Market
{
    public class MarketViewModel
    {
        public string RF01_CODE { get; set; }
        public string RF01_DESCRIPTION { get; set; }
        public string RF01_DESCRIPTION_SEC { get; set; }
        public int RF01_STATUS { get; set; }
        public int RF01_TYPE { get; set; }
        public int RF01_CREATED_BY { get; set; }
        public DateTime RF01_CREATED_DATE { get; set; }
        public int RF01_MODIFIED_BY { get; set; }
        public DateTime RF01_MODIFIED_DATE { get; set; }
        public int RF01_MARKET_ID { get; set; }
        public string RF01_IP { get; set; }
        public bool isActive { get; set; }
        public string RF01_REUTER_CODE { get; set; }
        public string RF01_START_TIME { get; set; }
        public string RF01_END_TIME { get; set; }
        public string RF01_WEEKEND { get; set; }
        public string[] Rf01_Weekend_Arr { get; set; }
        public string RF01_LANGUAGE { get; set; }
        public string RF01_CURRENCY { get; set; }
        public bool isPosted { get; set; }
        public int RF01_IS_POSTED { get; set; }
        public List<MarketViewModel> lstMarket { get; set; }
        public List<DropDowns> lstCurrency { get; set; }
    }
}
