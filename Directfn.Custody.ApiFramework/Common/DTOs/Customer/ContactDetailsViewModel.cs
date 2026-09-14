using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class ContactDetailsViewModel
    {
        public int crm03_id { get; set; }
        public int crm03_crm01_id { get; set; }
        public string crm03_contact_name { get; set; }
        public int crm03_main_contact { get; set; }
        public string crm03_email { get; set; }
        public string crm03_mobile_no { get; set; }
        public string crm03_fax_no { get; set; }
        public string crm03_work_no { get; set; }
        public string crm03_home_no { get; set; }
        public int crm03_created_by { get; set; }
        public DateTime crm03_created_date { get; set; }
        public int crm03_modified_by { get; set; }
        public DateTime crm03_modified_date { get; set; }
        public int crm03_status { get; set; }
        public string crm03_contact_name_sec { get; set; }
        public string crm03_ip { get; set; }
        public string crm03_postal_code { get; set; }
        public string crm03_title { get; set; }
        public string crm03_public_detail { get; set; }
    }
}
