using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.FOP
{
    public interface IFOPRepository
    {
        Task<List<FOPValidate>> GetFOP(DataSet result, int rf48_id, int portfolioId, int created_by, CancellationToken cancellationToken);
        Task<List<FOPValidate>> FOPStagingTable(List<FOPValidate> _list, int batch_id, int rf48_id, int portfolioId, int created_by, CancellationToken cancellationToken);
        Task<List<FOPValidate>> DistinctUniqueRef(List<FOPValidate> _list, CancellationToken cancellationToken);
        Task<List<FOPValidate>> GetValidFOPByBatchId(int batchId, CancellationToken cancellationToken);
        Task<string> SaveFOP(List<FOPValidate> _list, string MemberCode, CancellationToken cancellationToken);
    }
}
