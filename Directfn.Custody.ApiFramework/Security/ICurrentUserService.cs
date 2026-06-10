using System.Security.Claims;

namespace Directfn.Custody.ApiFramework.Security
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }
        string? UserId { get; }
        string? UserName { get; }
        string? SessionId { get; }
        string? Email { get; }
        IReadOnlyList<Claim> Claims { get; }
    }
}
