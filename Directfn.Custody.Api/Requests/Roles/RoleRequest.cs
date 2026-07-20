using Directfn.Custody.ApiFramework.DTOs;

namespace Directfn.Custody.Api.Requests.Roles
{
    public class RoleRequest
    {
        public RoleViewModel Role { get; set; }
        public List<string> Entitlements { get; set; } = new();
    }
}
