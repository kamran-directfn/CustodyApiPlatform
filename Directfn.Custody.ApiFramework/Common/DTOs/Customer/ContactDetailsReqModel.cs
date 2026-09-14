using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class ContactDetailsReqModel
    {
        public int crm03_id { get; set; }
        public int crm03_crm01_id { get; set; }
        public string crm03_home_no { get; set; }
        public string crm03_mobile_no { get; set; }
        public string crm03_email { get; set; }
        public string crm03_contact_name { get; set; }
        public string crm03_title { get; set; }
        public string crm03_fax_no { get; set; }
        public string crm03_public_detail { get; set; }
        public int crm03_created_by { get; set; }
    }
}
