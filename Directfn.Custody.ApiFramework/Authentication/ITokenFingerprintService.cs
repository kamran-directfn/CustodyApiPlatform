namespace Directfn.Custody.ApiFramework.Authentication
{
    public interface ITokenFingerprintService
    {
        TokenFingerprintResult Generate();

        string Hash(string fingerprint);
    }
}
