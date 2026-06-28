namespace Directfn.Custody.ApiFramework.DTOs.Entitlements;

public sealed class UserEntitlementRecord
{
    public string? GroupName { get; init; }

    public string? ScreenName { get; init; }

    public string? ControllerName { get; init; }

    public string? ActionName { get; init; }

    public string? Entitlement { get; init; }

    public string? UserName { get; init; }

    public string? SectionName { get; init; }

    public bool HasChild { get; init; }
}

public class UserRoleEntitlements
{
    public int Entitlement_Id { get; set; }
    public string group_name { get; set; }
    public string screen_name { get; set; }
    public string entitlement { get; set; }
    public bool isChecked { get; set; }
    public string parent_controller { get; set; }
    public string section { get; set; }

}
public class Group
{
    public Group()
    {
        screens = new List<Screen>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string JsSelector { get; set; }
    public List<Screen> screens { get; set; }
}

public class Screen
{
    public Screen()
    {
        screenEntitlements = new List<ScreenEntitlement>();
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string GroupName { get; set; }
    public string JsSelector { get; set; }
    public List<ScreenEntitlement> screenEntitlements { get; set; }

}

public class ScreenEntitlement
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool IsSelected { get; set; }

}

public class RolesEntitlements
{

    public string label { get; set; }
    public bool isChecked { get; set; }
    public string id { get; set; }
    public List<RolesEntitlements> items { get; set; }
    public bool expanded { get; set; }
}