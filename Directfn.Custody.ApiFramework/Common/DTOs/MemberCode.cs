using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs
{
    public class MemberCode
    {
        public int UM09_ID { get; set; }
        public int UM09_UM02_ID { get; set; }
        public int UM09_RF48_ID { get; set; }
        public int UM09_STATUS { get; set; }
        public int UM09_CREATED_BY { get; set; }
        public int UM09_MODIFIED_BY { get; set; }
        public DateTime UM09_CREATED_DATE { get; set; }
        public DateTime UM09_MODIFIED_DATE { get; set; }
        public string um02_name { get; set; }
        public string rf48_code { get; set; }
        public int um09_um14_id { get; set; }
    }
}
