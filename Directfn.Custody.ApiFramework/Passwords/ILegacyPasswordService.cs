namespace Directfn.Custody.ApiFramework.Passwords
{
    public interface ILegacyPasswordService
    {
        bool VerifyLegacyPassword(string providedPassword, string legacyEncryptedPassword);
        string EncryptLegacyPassword(string password);
    }
}
