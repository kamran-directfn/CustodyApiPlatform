using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Cities
{
    public class CitiesViewModel
    {
        public decimal RF10_CITY_ID { get; set; }
        public string RF10_DESCRIPTION { get; set; }
        public int RF10_CREATED_BY { get; set; }
        public DateTime RF10_CREATED_DATE { get; set; }
        public decimal RF10_MODIFIED_BY { get; set; }
        public DateTime RF10_MODIFIED_DATE { get; set; }
        public decimal RF10_STATUS_ID { get; set; }
        public decimal RF10_STATUS_CHANGED_BY { get; set; }
        public DateTime RF10_STATUS_CHANGED_DATE { get; set; }
        public decimal RF10_COUNTRY_ID { get; set; }
        public string Country_Desc { get; set; }
        public string RF10_EXTERNAL_REF { get; set; }
        public string RF10_DESCRIPTION_AR { get; set; }
        public decimal RF10_CMS_CITY_ID { get; set; }
        public decimal RF10_RF09_CMS_COUNTRY_ID { get; set; }
        public string RF10_IP_ADDRESS { get; set; }
        public int RF10_IS_POSTED { get; set; }
        public bool IsPosted { get; set; }
        public List<CitiesViewModel> lstCities { get; set; }
        public List<DropDowns> Countries { get; set; }
    }
}
