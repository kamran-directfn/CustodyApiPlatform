using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.Users
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            userPortfolioGroups = new List<UserPortfolioGroups>();
            memberCodes = new List<MemberCode>();
        }
        public int UM02_ID { get; set; }
        public string UM02_NAME { get; set; }
        public string UM02_LOGIN_ID { get; set; }
        public string UM02_PASSWORD { get; set; }
        public DateTime UM02_LAST_PWD_CHANGE { get; set; }
        public int UM02_SUPERVISOR { get; set; }
        public int UM02_IS_BRANCH_ADMIN { get; set; }
        public string UM02_JOB_TITLE { get; set; }
        public DateTime UM02_REG_DATE { get; set; }
        public DateTime UM02_EXPIRY_DATE { get; set; }
        public int UM02_BRANCH { get; set; }
        public int UM02_DEPARTMENT { get; set; }
        public string UM02_EMAIL { get; set; }
        public string UM02_MOBILE { get; set; }
        public string UM02_FAX { get; set; }
        public int UM02_UM01_ID { get; set; } ////////// password profile
        public int UM02_TYPE { get; set; }
        public string UM02_ASSIGNED_APPS { get; set; }
        public int UM02_AUTHENTICATION_MODE { get; set; }
        public int UM02_STATUS { get; set; }
        public DateTime UM02_LOCKED_DATE { get; set; }
        public DateTime UM02_LAST_LOGIN { get; set; }
        public int UM02_CREATED_BY { get; set; }
        public DateTime UM02_CREATED_DATE { get; set; }
        public int UM02_MODIFIED_BY { get; set; }
        public DateTime UM02_MODIFIED_DATE { get; set; }
        public string UM02_IMAGE { get; set; }
        public DateTime UM02_LAST_FAILED_LOGIN { get; set; }
        public string UM02_PREF_LANGUAGE { get; set; }
        public int UM02_FIRST_LOGIN { get; set; }
        public int UM02_LOCK_ACCOUNT { get; set; }
        public string UM02_REMARKS { get; set; }
        public int UM02_APPROVAL_STATUS { get; set; }
        public DateTime UM02_AUTHORIZED_DATE { get; set; }
        public int UM02_AUTHORIZED_BY { get; set; }
        public string UM02_IP { get; set; }
        public int[] memeberCode { get; set; }
        public int UM04_UM03_ID { get; set; } /////// for user role
        public List<UserViewModel> lstUsers { get; set; }
        public List<DropDowns> lstRoles { get; set; }
        public List<DropDowns> lstMemberCode { get; set; }
        public List<DropDowns> lstSupervisor { get; set; }
        public string ConfirmPassword { get; set; }
        public bool isPosted { get; set; }
        public int UM02_IS_POSTED { get; set; }
        public string Token { get; set; }
        public DateTime Token_Expiry { get; set; }
        public string suervisor { get; set; }
        public bool isGeneratePassword { get; set; }
        //public int um02_first_login { get; set; }
        public bool isFirstLogin { get; set; }
        public bool isLockAccount { get; set; }

        public List<UserEntitlements> lstEntitlements { get; set; }

        public string MemberCode { get; set; }
        public int MemberCodeID { get; set; }
        public string Modified_By_UserName { get; set; }
        public bool UM02_IS_LDAP { get; set; }
        public string Role_Name { get; set; }
        public string Last_Login_ { get; set; }
        public string Last_Login_Date { get; set; }
        public string User_Status { get; set; }
        public string UM02_LAST_NAME { get; set; }
        public string PostedStatus { get; set; }
        public string Expiry_Date { get; set; }
        public int UM09_UM14_ID { get; set; }
        public string UM14_GROUP_NAME { get; set; }
        public string um03_name { get; set; }
        public List<UserPortfolioGroups> userPortfolioGroups { get; set; }
        public List<MemberCode> memberCodes { get; set; }
        public int UM02_ATTEMPTNO { get; set; }
    }

    public class UserPortfolioGroups
    {
        public int GroupId { get; set; }
        public int MemberId { get; set; }
        public int UserId { get; set; }
        public int Member { get; set; }
    }
}