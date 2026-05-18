using Directfn.Custody.ApiFramework.Sessions;

namespace Directfn.Custody.SampleApi.Security;

public sealed class FakeAuthSessionService : IAuthSessionService
{
    public Task<bool> IsSessionValidAsync(
        string userId,
        string sessionId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}