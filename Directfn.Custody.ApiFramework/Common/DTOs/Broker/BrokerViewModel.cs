using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Broker
{
    public class BrokerViewModel
    {
        public int RF07_BROKER_ID { get; set; }
        public string RF07_REFERENCE_NO { get; set; }
        public int RF07_RF01_ID { get; set; }
        public string Market_Desc { get; set; }
        public decimal RF07_REPAID { get; set; }
        public string RF07_NAME { get; set; }
        public string RF07_NAME_SEC { get; set; }
        public string RF07_CONTACT_PERSON { get; set; }
        public string RF07_CONTACT_TITLE { get; set; }
        public string RF07_CONTACT_EMAIL { get; set; }
        public string RF07_CONTACT_PHONE { get; set; }
        public string RF07_CONTACT_FAX { get; set; }
        public string RF07_CONTACT_ADDRESS { get; set; }
        public int RF07_CREATED_BY { get; set; }
        public DateTime RF07_CREATED_DATE { get; set; }
        public int RF07_MODIFIED_BY { get; set; }
        public DateTime RF07_MODIFIED_DATE { get; set; }
        public string RF07_IP { get; set; }
        public int RF07_STATUS { get; set; }
        public int RF07_IS_POSTED { get; set; }
        public bool isPosted { get; set; }
        public string IsPosted_Desc { get; set; }
        public List<BrokerContacts> lstBrokerContacts { get; set; }
        public List<BrokerViewModel> lstBrokers { get; set; }
        public List<DropDowns> lstMarkets { get; set; }
        public string RF07_BIC_CODE { get; set; }
        public string RF07_TYPE { get; set; }
        public string RF07_PROP_ID { get; set; }
        public string RF07_CLIENT_POOL_ID { get; set; }
        public string RF07_PROP_SC_AC { get; set; }
        public string RF07_CLIENT_POOL_SCE { get; set; }
        public string RF07_REMARKS { get; set; }
    }
}
