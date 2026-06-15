namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public interface IAuthTokenStore
{
    Task CreateSessionAsync(AuthSessionRecord session, CancellationToken cancellationToken);

    Task StoreRefreshTokenAsync(RefreshTokenRecord refreshToken, CancellationToken cancellationToken);

    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken);

    Task RotateRefreshTokenAsync(string currentRefreshTokenHash, RefreshTokenRecord newRefreshToken, CancellationToken cancellationToken);

    Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken);

    Task<bool> IsSessionActiveAsync(string userId, string sessionId, CancellationToken cancellationToken);
}