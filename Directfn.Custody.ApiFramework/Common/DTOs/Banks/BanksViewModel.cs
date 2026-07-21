using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Banks
{
    public class BanksViewModel
    {
        public string RF03_NAME { get; set; }
        public string RF03_NAME_SEC { get; set; }
        public string RF03_CONTACT_NOS { get; set; }
        public string RF03_ADDRESS { get; set; }
        public string RF03_SWIFT_CODE { get; set; }
        public int RF03_INSTITUTION_ID { get; set; }
        public int RF03_CREATED_BY { get; set; }
        public DateTime RF03_CREATED_DATE { get; set; }
        public int RF03_MODIFIED_BY { get; set; }
        public DateTime RF03_MODIFIED_DATE { get; set; }
        public int RF03_STATUS { get; set; }
        public int RF03_IS_ISLAMIC { get; set; }
        public bool isIslamic { get; set; }
        public int RF03_ENABLE_ONLINE_TRANSFERS { get; set; }
        public bool isEnableOnlineTransfer { get; set; }
        public string RF03_EXTERNAL_REF { get; set; }
        public string RF03_CODE { get; set; }
        public int RF03_ID { get; set; }
        public string RF03_IP { get; set; }
        public int RF03_IS_POSTED { get; set; }
        public bool isPosted { get; set; }

        public List<BanksViewModel> lstBanks { get; set; }

    }
}
