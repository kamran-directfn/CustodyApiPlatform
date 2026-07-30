using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Currency;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Currency
{
    public interface ICurrencyRepository
    {
        Task<List<CurrencyViewModel>> GetAllCurrenciesAsync(CancellationToken cancellationToken);
        Task<CurrencyViewModel> GetCurrencyById(int currencyId, CancellationToken cancellationToken);
        Task<List<CurrencyViewModel>> UpdatePostStatus(int rf08_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<CurrencyViewModel>> DeleteCurrency(int rf08_id, int user_id, CancellationToken cancellationToken);
        Task<CurrencyReqModel> AddCurrency(CurrencyReqModel req, CancellationToken cancellationToken);
        Task<CurrencyReqModel> UpdateCurrency(CurrencyReqModel req, CancellationToken cancellationToken);
    }
}
