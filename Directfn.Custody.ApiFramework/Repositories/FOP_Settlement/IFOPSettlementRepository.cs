using Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_Settlement
{
    public interface IFOPSettlementRepository
    {
        Task<List<FOPSettlementViewModel>> GetFopSettlementAsync(PaginationRequest<FOPSettlementFilter> req, CancellationToken cancellationToken);

        byte[] ExportToExcel(List<FOPSettlementViewModel> data);
    }
}
