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