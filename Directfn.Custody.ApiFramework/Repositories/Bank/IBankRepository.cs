using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Bank
{
    public interface IBankRepository
    {
        Task<List<BanksViewModel>> GetAllBanksAsync(CancellationToken cancellationToken);
        Task<BanksViewModel> GetBankById(int bankId, CancellationToken cancellationToken);
        Task<List<BanksViewModel>> UpdatePostStatus(int rf03_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<BanksViewModel>> DeleteBank(int rf03_id, int user_id, CancellationToken cancellationToken);
        Task<BankReqModel> SaveBank(BankReqModel req, CancellationToken cancellationToken);
        Task<BankReqModel> UpdateBank(BankReqModel req, CancellationToken cancellationToken);
    }
}
