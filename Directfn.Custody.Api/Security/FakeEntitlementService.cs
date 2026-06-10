using Directfn.Custody.ApiFramework.Entitlements;

namespace Directfn.Custody.Api.Security
{
    public sealed class FakeEntitlementService : IEntitlementService
    {
        public Task<bool> HasAccessAsync(string userId, string controllerName, string actionName, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
