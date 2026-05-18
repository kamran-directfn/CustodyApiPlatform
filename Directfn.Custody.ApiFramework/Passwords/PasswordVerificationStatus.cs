namespace Directfn.Custody.ApiFramework.Passwords;

public enum PasswordVerificationStatus
{
    Failed = 0,
    Success = 1,
    SuccessRehashNeeded = 2
}