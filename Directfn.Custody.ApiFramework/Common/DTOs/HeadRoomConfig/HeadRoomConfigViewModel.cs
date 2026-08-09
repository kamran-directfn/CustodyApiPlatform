using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig
{
    public class HeadRoomConfigViewModel
    {
        public int headroomconfig_id { get; set; }
        public double SETTLEMENT_CAP { get; set; }
        public double LIMIT_UPDATE_EDAA { get; set; }
        public string SettlementCap { get; set; } = "Settlement Cap";
        public string Limit_UpdateEDAA { get; set; } = "Limit Update-EDAA";
    }
}
