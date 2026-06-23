namespace Directfn.Custody.ApiFramework.DTOs.Menus;

public sealed class LeftMenuGroupDto
{
    public string GroupLabel { get; init; } = default!;

    public bool Expanded { get; init; }

    public IReadOnlyList<LeftMenuItemDto> Items { get; init; } = [];
}