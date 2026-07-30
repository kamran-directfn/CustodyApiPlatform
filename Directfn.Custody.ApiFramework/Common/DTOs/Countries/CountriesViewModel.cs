using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Countries
{
    public class CountriesViewModel
    {
        public decimal RF09_COUNTRY_ID { get; set; }
        public string RF09_COUNTRY_NAME { get; set; }
        public string RF09_COUNTRY_CODE { get; set; }
        public string RF09_ARABIC_NAME { get; set; }
        public decimal RF09_CREATED_BY { get; set; }
        public DateTime RF09_CREATED_DATE { get; set; }
        public decimal RF09_MODIFIED_BY { get; set; }
        public DateTime? RF09_MODIFIED_DATE { get; set; }
        public decimal RF09_STATUS_ID { get; set; }
        public decimal RF09_STATUS_CHANGED_BY { get; set; }
        public DateTime RF09_STATUS_CHANGED_DATE { get; set; }
        public decimal RF09_NATIONALITY_CATEGORY { get; set; }
        public decimal RF09_IS_GCC { get; set; }
        public bool RF09_IS_GCC_bool { get; set; }
        public decimal RF09_VISIBLE { get; set; }
        public decimal RF09_CMS_COUNTRY_ID { get; set; }
        public decimal RF09_IS_ACTIVE { get; set; }
        public string RF09_IP_ADDRESS { get; set; }
        public decimal RF09_IS_POSTED { get; set; }
        public decimal Edited_by { get; set; }
        public bool isPosted { get; set; }
        public string Is_Posted { get; set; }
        public string Is_GCC { get; set; }
        public List<CountriesViewModel> lstCountries { get; set; }
    }
}
