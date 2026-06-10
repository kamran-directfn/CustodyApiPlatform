namespace Directfn.Custody.Api.Requests.User
{
    public sealed class LoginRequest
    {
        public string LoginId { get; init; } = default!;
        public string Password { get; init; } = default!; 

        public long Rf48Code { get; init; }
    }
}
