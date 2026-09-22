using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class StackeHolderReqModel
    {
        public int crm02_id { get; set; }
        public int crm02_crm01_id { get; set; }
        public int crm02_rf23_type_id { get; set; }
        public string crm02_number { get; set; }
        public string? CRM02_ISSUING_ENTITY { get; set; }
        public int crm02_created_by { get; set; }
        public DateTime crm02_created_date { get; set; }
        public int crm02_modified_by { get; set; }
        public DateTime crm02_modified_date { get; set; }

        public string? Type { get; set; }
    }
}
