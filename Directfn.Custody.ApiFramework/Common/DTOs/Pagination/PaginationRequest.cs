namespace Directfn.Custody.ApiFramework.Common.DTOs.Pagination
{

    public class PaginationRequest<TFilter>
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? filter { get; set; }
        public string? sort { get; set; }
        public TFilter? Filters { get; set; }
    }

    public class Sort
    {
        public string field { get; set; }
        public string dir { get; set; }
    }
    public class FilterClass
    {
        public string logic { get; set; }
        public List<FilterDescription> filters { get; set; }

    }

    public class FilterDescription
    {
        public string @operator { get; set; }
        public string field { get; set; }
        public string value { get; set; }
    }
    public class UserResult
    {
        public object list { get; set; }
        public int Total { get; set; }
    }

}