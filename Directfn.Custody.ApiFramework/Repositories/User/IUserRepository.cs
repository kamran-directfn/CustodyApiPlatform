using Directfn.Custody.ApiFramework.DTOs.User;

namespace Directfn.Custody.ApiFramework.Repositories.User
{
    public interface IUserRepository
    {
        Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken);
    }
}
