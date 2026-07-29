using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.MarketSector
{
    public class MarketSectorViewModel
    {
        public int RF40_ID { get; set; }
        public string RF40_NAME { get; set; }
        public string RF40_NAME_SEC { get; set; }
        public int RF40_STATUS { get; set; }
        public int RF40_RF01_ID { get; set; }
        public string RF40_CODE { get; set; }
        public int RF40_CREATED_BY { get; set; }
        public DateTime RF40_CREATED_DATE { get; set; }
        public int RF40_MODIFIED_BY { get; set; }
        public DateTime RF40_MODIFIED_DATE { get; set; }
        public string RF40_IP { get; set; }
        public int RF40_IS_POSTED { get; set; }
        public bool isPosted { get; set; }
        public List<DropDowns> lstMarkets { get; set; }
        public List<MarketSectorViewModel> lstMarketSectors { get; set; }
    }
}
