using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs
{
    public class BatchExecute
    {
        public int RF42_ID { get; set; }
        public string RF42_DESCRIPTION { get; set; }
        public string RF42_DESCRIPTION_SEC { get; set; }
        public string RF42_TYPE { get; set; }
        public int RF42_STATUS { get; set; }
        public string RF42_KIND { get; set; }
        public int RF42_CREATED_BY { get; set; }
        public DateTime RF42_CREATED_DATE { get; set; }
        public int RF42_MODIFIED_BY { get; set; }
        public DateTime RF42_MODIFIED_DATE { get; set; }
        public string RF42_IP { get; set; }
        public string RF42_TIME { get; set; }
        public int RF42_MEMBER_CODE_ID { get; set; }

    }
}
