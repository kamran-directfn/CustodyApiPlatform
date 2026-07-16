using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup
{
    public class PortfolioGroupReqModel
    {
        public int um14_id { get; set; }
        public string um14_group_name { get; set; }
        public string um14_group_description { get; set; }
        public int um14_rf48_id { get; set; }
        public int um14_created_by { get; set; }
        public int um14_updated_by { get; set; }

        public List<PortfolioGroupValidate> uploadedList { get; set; }
    }

}
