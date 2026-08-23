using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_EDAA
{
    public interface IFOP_EDAARepository
    {
        Task<List<FOPValidate>> GetAllFOP_EDAA_Async(PaginationRequest req, int memberCodeId, int groupId, CancellationToken cancellationToken);
    }
}
