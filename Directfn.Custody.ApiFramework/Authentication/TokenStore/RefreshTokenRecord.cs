namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public sealed class RefreshTokenRecord
{
    public string RefreshTokenId { get; init; } = default!;

    public string SessionId { get; init; } = default!;

    public string UserId { get; init; } = default!;

    public string TokenHash { get; init; } = default!;

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset ExpiresAtUtc { get; init; }

    public DateTimeOffset? UsedAtUtc { get; init; }

    public DateTimeOffset? RevokedAtUtc { get; init; }

    public string? ReplacedByTokenHash { get; init; }

    public string? CreatedByIp { get; init; }

    public string? UserAgent { get; init; }

    public bool IsActive => UsedAtUtc is null && RevokedAtUtc is null && ExpiresAtUtc > DateTimeOffset.UtcNow;
}