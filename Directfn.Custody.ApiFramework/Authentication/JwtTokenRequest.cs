namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class JwtTokenRequest
    {
        public string UserId { get; init; } = default!;

        public string UserName { get; init; } = default!;

        public string SessionId { get; init; } = default!;

        public string FingerprintHash { get; init; } = default!;

        public string? Email { get; init; }

        public IReadOnlyList<string> Roles { get; init; } = [];
    }
}
