using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Broker
{
    public class BrokerReqModel
    {
        public int RF07_BROKER_ID { get; set; }
        public string RF07_REFERENCE_NO { get; set; }
        public int RF07_RF01_ID { get; set; }
        public string RF07_NAME { get; set; }
        public string RF07_NAME_SEC { get; set; }
        public decimal RF07_REPAID { get; set; }
        public string RF07_TYPE { get; set; }
        public string RF07_PROP_ID { get; set; }
        public string RF07_CLIENT_POOL_SCE { get; set; }
        public string RF07_PROP_SC_AC { get; set; }
        public string RF07_BIC_CODE { get; set; }
        public string RF07_REMARKS { get; set; }
        public int RF07_CREATED_BY { get; set; }
        public int RF07_MODIFIED_BY { get; set; }

        public List<BrokerContacts> lstBrokerContact { get; set;  }
    }
}
