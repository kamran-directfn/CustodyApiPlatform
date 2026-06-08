namespace Directfn.Custody.ApiFramework.Entitlements
{
    public interface IEntitlementService
    {
        Task<bool> HasAccessAsync(string userId, string controllerName, string actionName, CancellationToken cancellationToken);
    }
}