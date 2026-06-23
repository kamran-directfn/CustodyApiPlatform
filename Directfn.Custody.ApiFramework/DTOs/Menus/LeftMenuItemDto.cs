namespace Directfn.Custody.ApiFramework.DTOs.Menus;

public sealed class LeftMenuItemDto
{
    public string Label { get; init; } = default!;

    public string Route { get; init; } = default!;

    public string Icon { get; init; } = default!;
}