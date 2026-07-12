using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Directfn.Custody.ApiFramework.Security;

internal sealed class CustodyUserContext : ICustodyUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustodyUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    private ClaimsPrincipal? User => HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public long? UserId => TryGetLong(ClaimTypes.NameIdentifier);

    public string? UserName => GetClaim(ClaimTypes.Name);

    public string? Email => GetClaim(ClaimTypes.Email);

    public string? SessionId => GetClaim("sid");

    public string? MemberCode => GetClaim("member_code");

    public long? MemberCodeId => TryGetLong("member_code_id");

    public string? IpAddress => HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? UserAgent => HttpContext?.Request.Headers.UserAgent.ToString();

    public string? CorrelationId => HttpContext?.Items["CorrelationId"]?.ToString();

    public IReadOnlyCollection<Claim> Claims => User?.Claims.ToList() ?? [];

    public bool IsInRole(string role)
    {
        return User?.IsInRole(role) ?? false;
    }

    public string? GetClaim(string claimType)
    {
        return User?.FindFirst(claimType)?.Value;
    }

    private long? TryGetLong(string claimType)
    {
        string? value = GetClaim(claimType);

        return long.TryParse(value, out long result)
            ? result
            : null;
    }
}