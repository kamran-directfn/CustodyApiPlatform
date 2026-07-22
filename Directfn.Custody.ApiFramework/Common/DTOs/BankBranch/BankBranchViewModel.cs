using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.BankBranch
{
    public class BankBranchViewModel
    {
        public string RF04_NAME { get; set; }
        public string RF04_ADDRESS { get; set; }
        public string RF04_TEL { get; set; }
        public int RF04_CREATED_BY { get; set; }
        public DateTime RF04_CREATED_DATE { get; set; }
        public int RF04_MODIFIED_BY { get; set; }
        public DateTime RF04_MODIFIED_DATE { get; set; }
        public string RF04_EXTERNAL_REF { get; set; }
        public int RF04_ID { get; set; }
        public int RF04_RF03_ID { get; set; }
        public int RF04_ACTIVE { get; set; }
        public string RF04_IP { get; set; }
        public string RF04_NAME_SEC { get; set; }
        public string RF04_CODE { get; set; }
        public int RF04_STATUS { get; set; }
        public int RF04_IS_POSTED { get; set; }
        public bool isActive { get; set; }
        public bool isPosted { get; set; }
        public List<BankBranchViewModel> lstBankBranches { get; set; }
        public List<DropDowns> lstBanks { get; set; }
    }
}
