using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;


namespace Directfn.Custody.ApiFramework.Repositories.User;

public interface IUserRepository
{
    Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken);

    Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserEntitlementRecord>> GetUserEntitlementsAsync(long userId, CancellationToken cancellationToken);
}