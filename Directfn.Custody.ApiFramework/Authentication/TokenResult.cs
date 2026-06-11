namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class TokenResult
    {
        public string AccessToken { get; init; } = default!;

        public DateTime ExpiresAtUtc { get; init; }

        public int ExpiresInSeconds { get; init; }

        public string TokenType { get; init; } = "Bearer";
        public bool Found { get; init; } = true;
        public bool FirstLogin { get; set; } = false;
    }
}
