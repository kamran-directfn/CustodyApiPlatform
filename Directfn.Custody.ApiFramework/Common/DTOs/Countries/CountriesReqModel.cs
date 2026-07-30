using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Countries
{
    public class CountriesReqModel
    {
        public int RF09_COUNTRY_ID { get; set; }
        public string RF09_COUNTRY_NAME { get; set; }
        public string RF09_COUNTRY_CODE { get; set; }
        public string RF09_ARABIC_NAME { get; set; }
        public int RF09_IS_GCC { get; set; }
        public int RF09_CREATED_BY { get; set; }
        public int RF09_MODIFIED_BY { get; set; }
        public string? ERROR_MESSAGE { get; set; }
    }
}
