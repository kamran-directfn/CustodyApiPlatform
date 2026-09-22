using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Customer
{
    public class InterestedPartyViewModel
    {
        public int CRM07_ID { get; set; }
        public int CRM07_CRM05_ID { get; set; }
        public string CRM07_TYPE { get; set; }
        public string CRM07_IDENTIFIER { get; set; }
        public string CRM07_IDENTIFERTYPE { get; set; }
        public int CRM07_STATUS { get; set; }
        public int CRM07_CREATED_BY { get; set; }
        public DateTime CRM07_CREATED_DATE { get; set; }
        public int CRM07_MODIFIED_BY { get; set; }
        public string identityname { get; set; }
        public string ninname { get; set; }
        public string IdentifierType { get; set; }

        public string Indentifer { get; set; }
        public string Type { get; set; }

        public DateTime CRM07_MODIFIED_DATE { get; set; }
    }
}
