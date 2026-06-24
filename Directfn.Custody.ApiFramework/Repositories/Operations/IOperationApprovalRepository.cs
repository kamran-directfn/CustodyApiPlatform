using Directfn.Custody.ApiFramework.DTOs.Approvals;

namespace Directfn.Custody.ApiFramework.Repositories.Operations;

public interface IOperationApprovalRepository
{
    Task<CanApproveDisapproveResponse?> CheckUserCanPerformOperationAsync(long userId, long memberCodeId, string screen, string recordIds, CancellationToken cancellationToken);
}