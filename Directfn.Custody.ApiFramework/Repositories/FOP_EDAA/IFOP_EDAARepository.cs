using Directfn.Custody.ApiFramework.Common.DTOs;
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
        Task<List<FOP_Child_Data>> GetFopEddaChild(string referenceNo, CancellationToken cancellationToken);
        Task<FOPValidate> ExportMT540Msg(int id, CancellationToken cancellationToken);
       // Task<Message> Get_Acknowledgment_Message(string id, CancellationToken cancellationToken);
    }
}
