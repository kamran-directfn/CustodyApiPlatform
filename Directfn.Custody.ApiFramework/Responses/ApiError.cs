namespace Directfn.Custody.ApiFramework.Responses
{
    public sealed class ApiError
    {
        public string Code { get; init; } = default!;
        public string Message { get; init; } = default!;
        public string? Field { get; init; }
    }
}
