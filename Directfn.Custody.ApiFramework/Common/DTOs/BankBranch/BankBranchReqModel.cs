using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.BankBranch
{
    public class BankBranchReqModel
    {
        public int RF04_ID { get; set; }
        public string RF04_CODE { get; set; }
        public string RF04_NAME { get; set; }
        public string RF04_NAME_SEC { get; set; }
        public int RF04_RF03_ID { get; set; }
        public string RF04_ADDRESS { get; set; }
        public string RF04_EXTERNAL_REF { get; set; }
        public int RF04_ACTIVE { get; set; }
        public string RF04_TEL { get; set; }
        public int RF04_CREATED_BY { get; set; }
        public int RF04_MODIFIED_BY { get; set; }
          
    }
}
