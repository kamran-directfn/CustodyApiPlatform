using System.Security.Claims;

namespace Directfn.Custody.ApiFramework.Security;

public interface ICustodyUserContext
{
    bool IsAuthenticated { get; }

    long UserId { get; }

    string UserName { get; }

    string Email { get; }

    string? SessionId { get; }

    string MemberCode { get; }

    long MemberCodeId { get; }
    long PortfolioGroupId { get; } 

    string? IpAddress { get; }

    string? UserAgent { get; }

    string? CorrelationId { get; }

    IReadOnlyCollection<Claim> Claims { get; }

    bool IsInRole(string role);

    string? GetClaim(string claimType);
}