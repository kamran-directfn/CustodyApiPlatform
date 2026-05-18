namespace Directfn.Custody.ApiFramework.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<ApiError> Errors { get; init; } = [];
    public string? CorrelationId { get; init; }

    public static ApiResponse<T> Ok(T data, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            CorrelationId = correlationId
        };
    }

    public static ApiResponse<T> Fail(
        IReadOnlyList<ApiError> errors,
        string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors,
            CorrelationId = correlationId
        };
    }
}