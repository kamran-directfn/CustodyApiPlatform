using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig
{
    public class BicCodeConfigViewModel
    {
        public int RF84_ID { get; set; }
        public string RF84_NAME { get; set; }
        public string RF84_NAME_SEC { get; set; }
        public string RF84_BIC_CODE { get; set; }
        public string RF84_CREATED_BY { get; set; }
        public int RF84_MODIFIED_BY { get; set; }
        public string RF84_STATUS { get; set; }
        public bool IsPosted { get; set; }
        public int RF84_IS_POSTED { get; set; }
        public List<BicCodeConfigViewModel> BicCodes { get; set; }
    }
}
