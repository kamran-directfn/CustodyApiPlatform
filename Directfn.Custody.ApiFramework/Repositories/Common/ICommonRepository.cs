using Directfn.Custody.ApiFramework.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Common
{
    public interface ICommonRepository
    {
        Task<List<DropDowns>> GetRoles(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetMemberCode(CancellationToken cancellationToken);
    }
}
