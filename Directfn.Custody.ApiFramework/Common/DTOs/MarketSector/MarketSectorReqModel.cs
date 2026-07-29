using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.MarketSector
{
    public class MarketSectorReqModel
    {
        public int RF40_ID { get; set; }
        public string RF40_NAME { get; set; }
        public string RF40_NAME_SEC { get; set; }
        public int RF40_RF01_ID { get; set; }
        public string RF40_CODE { get; set; }
        public int RF40_CREATED_BY { get; set; }
        public int RF40_MODIFIED_BY { get; set; }
        public string? ERROR_MESSAGE { get; set; }
    }
}
