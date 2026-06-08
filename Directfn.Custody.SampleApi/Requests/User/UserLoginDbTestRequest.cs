namespace Directfn.Custody.SampleApi.Requests.User
{
    public sealed class UserLoginDbTestRequest
    {
        public string LoginId { get; init; } = default!;

        public long Rf48Code { get; init; }
    }
}