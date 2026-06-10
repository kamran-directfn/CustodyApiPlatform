using Directfn.Custody.ApiFramework.Sessions;

namespace Directfn.Custody.Api.Security
{
    public sealed class FakeAuthSessionService : IAuthSessionService
    {
        public Task<bool> IsSessionValidAsync(string userId, string sessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
