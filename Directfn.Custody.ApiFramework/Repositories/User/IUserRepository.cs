using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.DTOs.Entitlements;
using Directfn.Custody.ApiFramework.DTOs.User;


namespace Directfn.Custody.ApiFramework.Repositories.User;

public interface IUserRepository
{
    Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, string rf48Code, CancellationToken cancellationToken);

    Task ChangeFirstLoginPasswordAsync(long userId, string encryptedPassword, CancellationToken cancellationToken);
    Task<List<UserViewModel>>GetAllUserAsync(CancellationToken cancellationToken);
    Task<UserViewModel>GetUserByIDAsync(int userId, CancellationToken cancellationToken);
    Task<string> VerifyUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<int> SaveUserAsync(UserRequestModel user);
    Task<string> UpdateUser(UserRequestModel user);
    Task<List<UserViewModel>> UpdatePostStatus(int um02_id, int isPosted, int user_id, CancellationToken cancellationToken);
    Task<string> Delete(int um02_id, int user_id, CancellationToken cancellationToken);
    Task SaveMemberCode(MemberCode code);
    Task DeleteMemberByUser(int um02_id);
    Task SaveSadminPortfoliosEntries(int um02_Id);
    Task<IReadOnlyList<UserEntitlementRecord>> GetUserEntitlementsAsync(long userId, CancellationToken cancellationToken);
    Task<MemberCodeRecord?> GetMemberCodeAsync(string memberCode, CancellationToken cancellationToken);
}