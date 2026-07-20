namespace Directfn.Custody.ApiFramework.Common.Database;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int TotalCount { get; init; }
}