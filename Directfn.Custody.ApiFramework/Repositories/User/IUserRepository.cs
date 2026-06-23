using Directfn.Custody.ApiFramework.DTOs.User;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;


namespace Directfn.Custody.ApiFramework.Repositories.User;

public interface IUserRepository
{
    Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken);
    Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken);
    Task<List<UserViewModel>>GetAllUserAsync(CancellationToken cancellationToken);
    Task<UserViewModel>GetUserByIDAsync(int userId, CancellationToken cancellationToken);
    Task VerifyUserNameAsync(string userName, CancellationToken cancellationToken);
    Task SaveUserAsync(UserViewModel user);
    Task UpdatePostStatus(int user_id, CancellationToken cancellationToken);
    Task Delete(int user_id, CancellationToken cancellationToken);

    Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserEntitlementRecord>> GetUserEntitlementsAsync(long userId, CancellationToken cancellationToken);
}