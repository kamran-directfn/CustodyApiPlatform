using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class CustomerBasicInfoReqModel
    {
        public int CRM01_ID { get; set; }
        public string? CRM01_EDAA_ID { get; set; }
        public int CRM01_ACTIVE { get; set; }
        public int CRM01_RF48_ID { get; set; }
        public string? CRM01_LONG_NAME { get; set; }
        public string? CRM01_FIRST_NAME { get; set; }
        public string? CRM01_ENG_LONG_NAME { get; set; }
        public string? CRM01_ENG_SHORT_NAME { get; set; }
        public int CRM01_TAX_RF09_ID { get; set; }
        public string? CRM01_TAX_IDENTIFIER { get; set; }
        public int CRM01_COMM_LANGUAGE { get; set; }
        public int CRM01_COMM_METHOD { get; set; }
        public int CRM01_INTERESTED_PARTY { get; set; }
        public string? CRM01_STAKEHOLDER_TYPE { get; set; }
        public DateTime? CRM01_DOB { get; set; }
        public int CRM01_RESIDENCE_RF09_ID { get; set; }
        public DateTime? CRM01_DECEASED_DATE { get; set; }
        public int CRM01_CITIZEN_RF09_ID { get; set; }
        public int CRM01_RF32_ID { get; set; }
        public string? CRM01_SECOND_NAME { get; set; }
        public string? CRM01_THIRD_NAME { get; set; }
        public string? CRM01_FOURTH_NAME { get; set; }
        public string? CRM01_HJIRI_DOB { get; set; }
        public int CRM01_GENDER { get; set; }
        public bool? CRM01_IS_TAX_EXEMPT { get; set; }
        public DateTime CRM01_EXEMPTION_VALID_UNTIL { get; set; }

        public int CRM01_LEGAL_STATUS { get; set; }
        public DateTime? CRM01_LEGAL_STATUS_DATE { get; set; }
        public int CRM01_REG_RF09_ID { get; set; }
        public DateTime? CRM01_INC_DATE { get; set; }

        public int CRM01_CREATED_BY { get; set; }
        public int CRM01_MODIFIED_BY { get; set; }

        public int CRM01_IS_EDAA_CREATED { get; set; }
    }
}
