namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public enum RefreshTokenValidationStatus
{
    Valid = 1,
    Missing = 2,
    NotFound = 3,
    Expired = 4,
    Revoked = 5,
    AlreadyUsed = 6,
    SessionInvalid = 7
}