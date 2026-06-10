namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class AuthOptions
    {
        public const string SectionName = "Authentication";

        public bool Enabled { get; init; }

        public string Issuer { get; init; } = default!;

        public string Audience { get; init; } = default!;

        public int AccessTokenMinutes { get; init; } = 10;

        public int RefreshTokenHours { get; init; } = 12;

        public string AccessTokenCookieName { get; init; } = "__Host-dfn-access";

        public string RefreshTokenCookieName { get; init; } = "__Host-dfn-refresh";

        public string FingerprintCookieName { get; init; } = "__Host-dfn-fp";

        public bool UseSecureCookies { get; init; } = true;

        public string? SigningKey { get; init; }

        public SigningCertificateOptions SigningCertificate { get; init; } = new();
    }
}
