using Directfn.Custody.ApiFramework.Common.DTOs.BankBranch;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.BankBranch
{
    public interface IBankBranchRepository
    {
        Task<List<BankBranchViewModel>> GetAllBranchesAsync(CancellationToken cancellationToken);
        Task<BankBranchViewModel> GetBranchById(int branchkId, CancellationToken cancellationToken);
        Task<List<BankBranchViewModel>> UpdatePostStatus(int rf04_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<BankBranchViewModel>> DeleteBank(int rf04_id, int user_id, CancellationToken cancellationToken);
        Task<BankBranchReqModel> SaveBank(BankBranchReqModel req, CancellationToken cancellationToken);
        Task<BankBranchReqModel> UpdateBank(BankBranchReqModel req, CancellationToken cancellationToken);
    }
}
