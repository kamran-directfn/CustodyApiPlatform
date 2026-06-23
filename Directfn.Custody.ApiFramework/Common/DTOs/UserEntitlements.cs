using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs
{
    public class UserEntitlements
    {
        public string GROUP_NAME { get; set; }
        public string SCREEN_NAME { get; set; }
        public string CONTROLLER_NAME { get; set; }
        public string ACTION_NAME { get; set; }
        public string ENTITLEMENT { get; set; }
        public string USER_NAME { get; set; }
        //////// this property is used for tabs
        public string section_name { get; set; }
        public bool HasChild { get; set; }
    }
}
