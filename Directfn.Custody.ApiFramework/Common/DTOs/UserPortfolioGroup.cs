using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs
{
    public class UserPortfolioGroup
    {
    }

    public class PortfoliosByMembers
    {
        public int rf48_id { get; set; }
        public string rf48_code { get; set; }
        public string um14_group_name { get; set; }
        public int um14_id { get; set; }
        public int um14_group_hidden { get; set; }

    }
}
