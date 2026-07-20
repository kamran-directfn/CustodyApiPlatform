using System;
using System.Collections.Generic;
using System.Text;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
namespace Directfn.Custody.ApiFramework.DTOs
{
    public class RoleViewModel
    {
        public int UM03_ID { get; set; }
        public int UM03_PARENT_ID { get; set; }
        public string UM03_NAME { get; set; }
        public string UM03_NAME_SEC { get; set; }
        public int UM03_STATUS { get; set; }
        public int UM03_CREATED_BY { get; set; }
        public DateTime UM03_CREATED_DATE { get; set; }
        public int UM03_MODIFIED_BY { get; set; }
        public DateTime UM03_MODIFIED_DATE { get; set; }
        public string UM03_IP { get; set; }
        public bool isPosted { get; set; }
        public int UM03_IS_POSTED { get; set; }

        public List<UserRoleEntitlements> Entitlments { get; set; }

    }
  
}
