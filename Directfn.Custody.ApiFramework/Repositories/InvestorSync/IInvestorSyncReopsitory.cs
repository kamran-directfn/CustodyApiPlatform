using Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;

namespace Directfn.Custody.ApiFramework.Repositories.InvestorSync
{
    public interface IInvestorSyncReopsitory
    {
        Task<List<InvestorSyncViewModel>> GetData(PaginationRequest<InvestorSyncFilters> req, CancellationToken cancellationToken);
        Task<InvestorSyncViewModel> ExportReqMessage(string messageId, CancellationToken cancellationToken);
        Task<InvestorSyncViewModel> ExportResMessage(string messageId, CancellationToken cancellationToken);
        Byte[] ExportToExcel(List<InvestorSyncViewModel> data);
        Task<string> GetEdaaMsg(string messageIds, int memberCodeId, CancellationToken cancellationToken);
    }
}
