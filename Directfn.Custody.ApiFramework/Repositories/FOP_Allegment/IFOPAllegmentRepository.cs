using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_Allegment
{
    public interface IFOPAllegmentRepository
    {
        Task<List<FOPAllegmentViewModel>> GetFopAllegmentAsync(PaginationRequest<FOPAllegmentFilter> req, int rf48_id, int portfolioId, CancellationToken cancellationToken);
        Task<List<FOPAllegmentViewModel>> GetFopAllegmentChild(string referenceNo, CancellationToken cancellationToken);
        Task<FOPAllegmentViewModel> ExportMT578Msg(int id, CancellationToken cancellationToken);
    }
}
