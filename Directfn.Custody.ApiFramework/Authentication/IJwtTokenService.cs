namespace Directfn.Custody.ApiFramework.Authentication
{
    public interface IJwtTokenService
    {
        TokenResult GenerateAccessToken(JwtTokenRequest request);
    }
}