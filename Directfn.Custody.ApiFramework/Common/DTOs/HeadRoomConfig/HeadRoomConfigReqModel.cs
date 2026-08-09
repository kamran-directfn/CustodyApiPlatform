using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig
{
    public class HeadRoomConfigReqModel
    {
        public double SETTLEMENT_CAP { get; set; }
        public double LIMIT_UPDATE_EDAA { get; set; }
    }
}
