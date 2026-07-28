using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Cities
{
    public class CitiesReqModel
    {
        public int RF10_CITY_ID { get; set; }
        public string RF10_DESCRIPTION { get; set; }
        public int RF10_COUNTRY_ID { get; set; }
        public string RF10_DESCRIPTION_AR { get; set; }
        public int RF10_CREATED_BY { get; set; }
        public int RF10_MODIFIED_BY { get; set; }
    }
}
