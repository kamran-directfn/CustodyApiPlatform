namespace Directfn.Custody.ApiFramework.Passwords
{
    public interface IPasswordHashService
    {
        string HashPassword(string password);

        PasswordVerificationStatus VerifyPassword(string passwordHash, string providedPassword);
    }
}
