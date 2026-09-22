using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync
{
    public class InvestorSyncViewModel
    {
        public string crm01_messageid { get; set; }
        public string crm01_investorid { get; set; }
        public string CRM01_LONGNAME { get; set; }
        public string CRM01_FIRSTNAME { get; set; }
        public string crm01_activationtype { get; set; }
        public string CRM01_STACKEHOLDERTYPE { get; set; }
        public string crm01_message { get; set; }
        public int CRM01_ISSYNCED { get; set; }
        public int CRM01_IS_AVAIL_SYNC { get; set; }
        public int CRM01_RF48_ID { get; set; }
        public int CRM01_SYNC_ID { get; set; }
        public string CRM01_REQ_MESSAGE { get; set; }
        public int crm01_batch_id { get; set; }
        public int TotalRecords { get; set; }
        public string IsSync { get { return this.CRM01_ISSYNCED == 1 ? "Yes" : "No"; } }
        public string IsAvailSync { get { return this.CRM01_IS_AVAIL_SYNC == 1 ? "Yes" : "No"; } }
    }

    public class InvestorSyncFilters
    {
        public string? IsSync { get; set; }
        public string? crm01_investorid { get; set; }
        public int memberCodeId { get; set; }
    }
    
}
