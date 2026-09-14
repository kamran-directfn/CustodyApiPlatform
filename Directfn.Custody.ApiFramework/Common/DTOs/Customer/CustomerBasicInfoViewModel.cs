using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class CustomerBasicInfoViewModel
    {
        public int CRM01_ID { get; set; }
        public string CRM01_CRM_NO { get; set; }
        public int CRM01_TYPE { get; set; }
        public int CRM01_ACTIVE { get; set; }
        public int CRM01_PARENT_ID { get; set; }
        public int CRM01_RF48_ID { get; set; }
        public string CRM01_LONG_NAME { get; set; }
        public string CRM01_FIRST_NAME { get; set; }
        public string CRM01_ENG_LONG_NAME { get; set; }
        public string CRM01_ENG_SHORT_NAME { get; set; }
        public int CRM01_TAX_RF09_ID { get; set; }
        public string CRM01_TAX_IDENTIFIER { get; set; }
        public int CRM01_COMM_LANGUAGE { get; set; }
        public int CRM01_COMM_METHOD { get; set; }
        public int CRM01_INTERESTED_PARTY { get; set; }
        public string CRM01_STAKEHOLDER_TYPE { get; set; }
        public DateTime CRM01_DOB { get; set; }
        public int CRM01_RESIDENCE_RF09_ID { get; set; }
        public DateTime CRM01_DECEASED_DATE { get; set; }
        public int CRM01_CITIZEN_RF09_ID { get; set; }
        public string CRM01_SECOND_NAME { get; set; }
        public string CRM01_THIRD_NAME { get; set; }
        public string CRM01_FOURTH_NAME { get; set; }
        public string CRM01_HJIRI_DOB { get; set; }
        public int CRM01_GENDER { get; set; }
        public string CRM01_SH_INVESTOR_ID { get; set; }
        public int CRM01_IS_INDIVIDUAL { get; set; }
        public int CRM01_IS_SYNC { get; set; }
        public int CRM01_STATUS { get; set; }
        public int CRM01_CREATED_BY { get; set; }
        public DateTime CRM01_CREATED_DATE { get; set; }
        public int CRM01_MODIFIED_BY { get; set; }
        public DateTime CRM01_MODIFIED_DATE { get; set; }
        public int CRM01_RF32_ID { get; set; }
        public int CRM01_RF66_ID { get; set; }
        public int CRM01_LEGAL_STATUS { get; set; }
        public DateTime CRM01_LEGAL_STATUS_DATE { get; set; }
        public int CRM01_REG_RF09_ID { get; set; }
        public DateTime CRM01_INC_DATE { get; set; }
        public string CRM01_MESSAGE_ID { get; set; }
        public string CRM01_EDAA_ID { get; set; }
        public string CRM01_EDAA_STATUS { get; set; }
        public string member_code { get; set; }
        public string tax_country { get; set; }
        public string res_country { get; set; }
        public string citi_country { get; set; }
        public string customer_type { get; set; }
        public string CRM01_ACTIVE_Desc { get; set; }
        public string Req_Msg { get; set; }
        public string res_msg { get; set; }
        public int CRM01_IS_EDAA_CREATED { get; set; }
        public int TotalRecords { get; set; }
        public bool CRM01_IS_TAX_EXEMPT { get; set; }
        public DateTime CRM01_EXEMPTION_VALID_UNTIL { get; set; }
    }

    public class CustomerBasicInfoFilter 
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int memberCodeId { get; set; }
        public int PortfolioGroupId { get; set; }
        public string? Crm01_Active { get; set; }
        public string? Crm01_Edaa_Id { get; set; }
        public string? Investor_Category { get; set; }
        public string? Long_Name { get; set; }

    }
}
