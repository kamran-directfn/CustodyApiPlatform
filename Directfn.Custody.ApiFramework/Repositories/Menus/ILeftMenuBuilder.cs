using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.DTOs.Menus;

namespace Directfn.Custody.ApiFramework.Menus;

public interface ILeftMenuBuilder
{
    IReadOnlyList<LeftMenuGroupDto> Build(IReadOnlyList<UserEntitlementRecord> entitlements);
}