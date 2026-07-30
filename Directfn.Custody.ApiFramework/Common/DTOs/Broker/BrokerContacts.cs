using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Broker
{
    public class BrokerContacts
    {
        public int RF47_ID { get; set; }
        public int RF47_RF07_ID { get; set; }
        public string RF07_ACCOUNT { get; set; }
        public string RF07_ACCOUNT_TITLE { get; set; }
        public DateTime RF47_CREATED_DATE { get; set; }
        public int RF47_CREATED_BY { get; set; }
        public int RF47_MODIFIED_BY { get; set; }
        public DateTime RF47_MODIFIED_DATE { get; set; }
        public int RF47_STATUS { get; set; }
        public int RF47_IS_FROM_FILE { get; set; }
    }

    public class BrokerCache
    {
        public string BrokerBic { get; set; }
        public string BrokerCode { get; set; }

    }
}
