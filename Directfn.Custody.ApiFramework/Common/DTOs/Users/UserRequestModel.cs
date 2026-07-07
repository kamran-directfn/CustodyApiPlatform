using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Users
{
    public class UserRequestModel
    {
        public int UM02_ID { get; set; }
        [Required]
        public string UM02_LOGIN_ID { get; set; }
        [Required]
        public string UM02_NAME { get; set; }
        public string UM02_LAST_NAME { get; set; }
        public string UM02_FAX { get; set; }
        public string UM02_MOBILE { get; set; }
        [Required]
        public string UM02_EMAIL { get; set; }
        [Required]
        public int UM04_UM03_ID { get; set; } /////// for user role
        public DateTime UM02_REG_DATE { get; set; }
        public DateTime UM02_EXPIRY_DATE { get; set; }
        public string UM02_JOB_TITLE { get; set; }
        public int UM02_SUPERVISOR { get; set; }
        public int UM02_GENDER { get; set; }
       // public List<DropDowns> lstMemberCode { get; set; }
        public bool isLockAccount { get; set; }
        public bool UM02_IS_LDAP { get; set; }
        public bool isGeneratePassword { get; set; }
        //public string UM02_IMAGE { get; set; }

        public List<UserPortfolioGroups> userPortfolioGroups { get; set; }

    } 
}
