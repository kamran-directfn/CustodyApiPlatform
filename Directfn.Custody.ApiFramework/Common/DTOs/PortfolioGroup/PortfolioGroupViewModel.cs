using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup
{
    public class PortfolioGroupViewModel
    {
        public int um14_id { get; set; }
        public string um14_group_name { get; set; }
        public string um14_group_description { get; set; }
        public int um14_rf48_id { get; set; }
        public string um14_group_hidden { get; set; }
        public int um14_created_by { get; set; }
        public DateTime um14_created_date { get; set; }
        public int um14_updated_by { get; set; }
        public DateTime um14_updated_date { get; set; }
        public string created_date { get { return this.um14_created_date.ToString(); } }
        public string updated_date { get { return this.um14_updated_date.ToString(); } }
    }

    public class PortfolioFile
    {
        public string Account { get; set; }
        public bool isValid { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    }
    public class PortfolioGroupValidate
    {
        public int UM15_ID { get; set; }
        public string UM15_SECURITY_ACCOUNT { get; set; }
        public DateTime UM15_CREATED_DATE { get; set; }
        public int UM15_BATCH_ID { get; set; }
        public string UM15_STATUS { get; set; }
        public string UM15_REMARKS { get; set; }
        public int UM15_UM14_ID { get; set; }
        public int UM15_VALID { get; set; }
        public int UM14_RF48_ID { get; set; }
        public int UM15_CREATED_BY { get; set; }
        public int UM15_RF48_ID { get; set; }
    }

    public class PortfolioGroup_Details
    {
        public int UM15_ID { get; set; }
        public string UM15_SECURITY_ACCOUNT { get; set; }
        public DateTime UM15_CREATED_DATE { get; set; }

        public int UM15_CREATED_BY { get; set; }

        public int UM15_UM14_ID { get; set; }
        public int UM15_CRM05_ID { get; set; }


        public int UM15_RF48_ID { get; set; }
        public int UM15_UPDATED_BY { get; set; }
        public DateTime UM15_UPDATED_DATE { get; set; }
    }

    public class PortfolioGroupById
    {
        public PortfolioGroupViewModel Group { get; set; }
        public List<PortfolioGroup_Details> Group_Details { get; set; }
    }

    public class PortfolioGroupDeleteRes
    {
        public string feedBack { get; set; }


    }
}
