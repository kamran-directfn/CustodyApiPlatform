using System.Text.RegularExpressions;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.DTOs.Menus;

namespace Directfn.Custody.ApiFramework.Menus;

public sealed class LeftMenuBuilder : ILeftMenuBuilder
{
    public IReadOnlyList<LeftMenuGroupDto> Build(IReadOnlyList<UserEntitlementRecord> entitlements)
    {
        var validEntitlements = entitlements
            .Where(x => !string.IsNullOrWhiteSpace(x.GroupName))
            .Where(x => !string.IsNullOrWhiteSpace(x.ScreenName))
            .ToList();

        var groups = validEntitlements
            .GroupBy(x => CleanText(x.GroupName!))
            .Select((group, index) => new LeftMenuGroupDto
            {
                GroupLabel = group.Key,
                Expanded = index == 0,
                Items = group
                    .GroupBy(x => CleanText(x.ScreenName!))
                    .Select(itemGroup =>
                    {
                        string label = itemGroup.Key;

                        return new LeftMenuItemDto
                        {
                            Label = label,
                            Route = $"/app/screen/{ToKebabCase(label)}",
                            Icon = GetIcon(group.Key)
                        };
                    })
                    .OrderBy(x => x.Label)
                    .ToList()
            })
            .Where(x => x.Items.Count > 0)
            .OrderBy(x => x.GroupLabel == "Dashboard" ? 0 : 1)
            .ThenBy(x => x.GroupLabel)
            .ToList();

        return groups;
    }

    private static string CleanText(string value)
    {
        return value.Replace("\r", " ").Replace("\n", " ").Trim();
    }

    private static string ToKebabCase(string value)
    {
        string cleaned = CleanText(value).ToLowerInvariant();

        cleaned = cleaned.Replace("&", "and");

        cleaned = Regex.Replace(cleaned, @"[^a-z0-9]+", "-");

        cleaned = Regex.Replace(cleaned, @"-+", "-");

        return cleaned.Trim('-');
    }

    private static string GetIcon(string groupName)
    {
        string normalized = groupName.Trim().ToLowerInvariant();

        return normalized switch
        {
            "dashboard" => "pi pi-home",
            "user management" => "pi pi-users",
            "customer" => "pi pi-user",
            "definitions" => "pi pi-cog",
            "pledge" => "pi pi-lock",
            "transactions" => "pi pi-sync",
            "iso monitor" => "pi pi-desktop",
            "statement" => "pi pi-file",
            _ => "pi pi-folder"
        };
    }
}