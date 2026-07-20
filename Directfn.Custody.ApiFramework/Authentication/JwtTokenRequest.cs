namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class JwtTokenRequest
    {
        public string UserId { get; init; } = default!;

        public string UserName { get; init; } = default!;

        public string SessionId { get; init; } = default!;

        public string FingerprintHash { get; init; } = default!;

        public string? Email { get; init; }

        public string? MemberCode { get; init; }

        public string? MemberCodeId { get; init; }
        public IReadOnlyList<string> Roles { get; init; } = [];

        public string RoleName { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string ImageURL { get; init; } = default!;


    }
}
