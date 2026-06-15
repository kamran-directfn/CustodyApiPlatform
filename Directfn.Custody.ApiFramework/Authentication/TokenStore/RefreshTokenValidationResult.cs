namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public sealed class RefreshTokenValidationResult
{
    public RefreshTokenValidationStatus Status { get; init; }

    public AuthSessionRecord? Session { get; init; }

    public RefreshTokenRecord? RefreshToken { get; init; }

    public bool IsValid => Status == RefreshTokenValidationStatus.Valid;

    public static RefreshTokenValidationResult Valid(AuthSessionRecord session, RefreshTokenRecord refreshToken)
    {
        return new RefreshTokenValidationResult
        {
            Status = RefreshTokenValidationStatus.Valid,
            Session = session,
            RefreshToken = refreshToken
        };
    }

    public static RefreshTokenValidationResult Fail(RefreshTokenValidationStatus status)
    {
        return new RefreshTokenValidationResult
        {
            Status = status
        };
    }
}