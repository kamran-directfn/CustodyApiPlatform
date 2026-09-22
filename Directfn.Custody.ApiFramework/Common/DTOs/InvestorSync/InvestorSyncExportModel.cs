using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync
{
    public class InvestorSyncExportModel
    {
        public string crm01_investorid { get; set; }
        public string CRM01_LONGNAME { get; set; }
        public string CRM01_FIRSTNAME { get; set; }
        public string crm01_activationtype { get; set; }
        public string CRM01_STACKEHOLDERTYPE { get; set; }
        public string CRM01_ISSYNCED { get; set; }
        public string CRM01_IS_AVAIL_SYNC { get; set; }
        public int CRM01_SYNC_ID { get; set; }
        public string crm01_message { get; set; }
        public string CRM01_REQ_MESSAGE { get; set; }
    }
}
