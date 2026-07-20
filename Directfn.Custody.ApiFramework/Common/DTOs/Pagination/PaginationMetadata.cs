namespace Directfn.Custody.ApiFramework.Common.DTOs.Pagination;

public sealed class PaginationMetadata
{
    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}