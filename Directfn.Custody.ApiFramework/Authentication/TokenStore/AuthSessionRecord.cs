namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public sealed class AuthSessionRecord
{
    public string SessionId { get; init; } = default!;

    public string UserId { get; init; } = default!;

    public string? UserName { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }

    public DateTimeOffset? RevokedAtUtc { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTimeOffset.UtcNow;
}