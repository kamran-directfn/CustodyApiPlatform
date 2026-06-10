namespace Directfn.Custody.ApiFramework.Authentication
{
    public sealed class TokenResult
    {
        public string AccessToken { get; init; } = default!;

        public DateTime ExpiresAtUtc { get; init; }

        public int ExpiresInSeconds { get; init; }

        public string TokenType { get; init; } = "Bearer";
        public bool found { get; init; } = true;   
    }
}
