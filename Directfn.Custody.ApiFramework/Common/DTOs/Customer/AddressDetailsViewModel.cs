using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
   public class AddressDetailsViewModel
    {
        public int crm04_id { get; set; }
        public int crm04_crm01_id { get; set; }
        public string crm04_address1 { get; set; }
        public string crm04_address2 { get; set; }
        public string crm04_address3 { get; set; }
        public string crm04_postalcode { get; set; }
        public int crm04_rf09_id { get; set; }
        public string crm04_stateprovince { get; set; }
        public int crm04_rf10_id { get; set; }
        public int crm04_addresstype { get; set; }
        public int crm04_status { get; set; }
        public int crm04_created_by { get; set; }
        public DateTime crm04_created_date { get; set; }
        public int crm04_modified_by { get; set; }
        public DateTime crm04_modified_date { get; set; }

        public string Comments { get; set; }
        public string city { get; set; }
        public string country_code { get; set; }
    }
}
