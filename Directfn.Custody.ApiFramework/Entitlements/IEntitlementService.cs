namespace Directfn.Custody.ApiFramework.Entitlements
{
    public interface IEntitlementService
    {
        Task<bool> HasAccessAsync(long userId, string controllerName, string actionName, CancellationToken cancellationToken);
    }
}
