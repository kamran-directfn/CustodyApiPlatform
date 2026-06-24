using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;


namespace Directfn.Custody.ApiFramework.Repositories.User;

public interface IUserRepository
{
    Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, string rf48Code, CancellationToken cancellationToken);

    Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserEntitlementRecord>> GetUserEntitlementsAsync(long userId, CancellationToken cancellationToken);
    Task<MemberCodeRecord?> GetMemberCodeAsync(string memberCode, CancellationToken cancellationToken);
}