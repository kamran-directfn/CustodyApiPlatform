using Directfn.Custody.ApiFramework.Entitlements;

namespace Directfn.Custody.SampleApi.Security;

public sealed class FakeEntitlementService : IEntitlementService
{
    public Task<bool> HasAccessAsync(
        string userId,
        string controllerName,
        string actionName,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}