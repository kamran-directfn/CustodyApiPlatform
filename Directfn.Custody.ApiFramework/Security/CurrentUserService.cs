using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Directfn.Custody.ApiFramework.Security
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public string? UserId =>
            User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? User?.FindFirstValue("sub") ?? User?.FindFirstValue("user_id");

        public string? UserName =>
            User?.FindFirstValue(ClaimTypes.Name) ?? User?.FindFirstValue("username") ?? User?.Identity?.Name;

        public string? SessionId =>
            User?.FindFirstValue("sid") ?? User?.FindFirstValue("session_id");

        public string? Email =>
            User?.FindFirstValue(ClaimTypes.Email) ?? User?.FindFirstValue("email");

        public IReadOnlyList<Claim> Claims =>
            User?.Claims.ToList() ?? [];
    }
}