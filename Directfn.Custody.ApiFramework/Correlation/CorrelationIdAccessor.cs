using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Correlation
{
    public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? CorrelationId =>
            _httpContextAccessor.HttpContext?.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
    }
}
