namespace Directfn.Custody.ApiFramework.Authentication;

public sealed class RefreshTokenPayload
{
    public string UserId { get; init; } = default!;
    public string? UserName { get; init; }
    public string? SessionId { get; init; }
    public string? Email { get; init; }
    public string? MemberCode { get; init; }

    public string? MemberCodeId { get; init; }
    public string? FingerprintHash { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public string RoleName { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string ImageURL { get; init; } = default!;
}