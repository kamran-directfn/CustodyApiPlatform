using Microsoft.AspNetCore.Identity;

namespace Directfn.Custody.ApiFramework.Passwords
{
    public sealed class AspNetPasswordHashService : IPasswordHashService
    {
        private readonly PasswordHasher<object> _passwordHasher = new();

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.", nameof(password));
            }

            return _passwordHasher.HashPassword(new object(), password);
        }

        public PasswordVerificationStatus VerifyPassword(string passwordHash, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(providedPassword))
            {
                return PasswordVerificationStatus.Failed;
            }

            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, providedPassword);

            return result switch
            {
                PasswordVerificationResult.Success => PasswordVerificationStatus.Success,

                PasswordVerificationResult.SuccessRehashNeeded => PasswordVerificationStatus.SuccessRehashNeeded,

                _ => PasswordVerificationStatus.Failed
            };
        }
    }
}
