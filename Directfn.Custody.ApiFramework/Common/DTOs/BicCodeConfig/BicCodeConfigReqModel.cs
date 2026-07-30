using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig
{
    public class BicCodeConfigReqModel
    {
        public int RF84_ID { get; set; }
        public string RF84_NAME { get; set; }
        public string RF84_NAME_SEC { get; set; }
        public string RF84_BIC_CODE { get; set; }
        public int RF84_CREATED_BY { get; set; }
        public int RF84_MODIFIED_BY { get; set; }
    }
}
