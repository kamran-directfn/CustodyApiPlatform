using Directfn.Custody.ApiFramework.Common.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Common
{
    public interface ICommonRepository
    {
        Task<List<DropDowns>> GetRoles(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetMemberCode(CancellationToken cancellationToken);
        Task<List<PortfoliosByMembers>> GetGroupsByMember(CancellationToken cancellationToken);
        DataSet UploadFileDataSet(IFormFile file);
        Task<int> GetBatchID(string ScreenName, CancellationToken cancellationToken);
        Task<List<DropDowns>> GetBanks(CancellationToken cancellationToken);
    }
}
