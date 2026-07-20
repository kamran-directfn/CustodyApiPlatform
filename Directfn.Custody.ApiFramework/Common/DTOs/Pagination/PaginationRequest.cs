namespace Directfn.Custody.ApiFramework.Common.DTOs.Pagination;

public sealed class PaginationRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}