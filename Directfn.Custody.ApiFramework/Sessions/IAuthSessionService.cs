namespace Directfn.Custody.ApiFramework.Sessions
{
    public interface IAuthSessionService
    {
        Task<bool> IsSessionValidAsync(string userId, string sessionId, CancellationToken cancellationToken);
    }
}
