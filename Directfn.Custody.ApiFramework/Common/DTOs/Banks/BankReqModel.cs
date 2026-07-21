using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Banks
{
    public class BankReqModel
    {
        public int RF03_ID { get; set; }
        public string RF03_CODE { get; set; }
        public string RF03_NAME { get; set; }
        public string RF03_NAME_SEC { get; set; }
        public string RF03_CONTACT_NOS { get; set; }
        public string RF03_SWIFT_CODE { get; set; }
        public string RF03_EXTERNAL_REF { get; set; }
        public int RF03_IS_ISLAMIC { get; set; }
        public int RF03_ENABLE_ONLINE_TRANSFERS { get; set; }
        public string RF03_ADDRESS { get; set; }
        public int RF03_CREATED_BY { get; set; }
        public int RF03_MODIFIED_BY { get; set; }
        
    }
}
