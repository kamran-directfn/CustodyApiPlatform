using Directfn.Custody.ApiFramework.Authentication.TokenStore;

namespace Directfn.Custody.ApiFramework.Sessions;

public sealed class AuthSessionService : IAuthSessionService
{
    private readonly IAuthTokenStore _authTokenStore;

    public AuthSessionService(IAuthTokenStore authTokenStore)
    {
        _authTokenStore = authTokenStore;
    }

    public Task<bool> IsSessionValidAsync(string userId, string sessionId, CancellationToken cancellationToken)
    {
        return _authTokenStore.IsSessionActiveAsync(userId, sessionId, cancellationToken);
    }
}