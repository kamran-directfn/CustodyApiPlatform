namespace Directfn.Custody.ApiFramework.Authentication;

public interface IRefreshTokenService
{
    string GenerateRefreshToken(JwtTokenRequest request);
    RefreshTokenPayload? ValidateRefreshToken(string refreshToken);
}