
using Directfn.Custody.ApiFramework.Common.DTOs.Market;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Market
{
    public interface IMarketRepository
    {
        Task<List<MarketViewModel>> GetAllMarketsAsync(CancellationToken cancellationToken);
        Task<MarketViewModel> GetMarketById(int currencyId, CancellationToken cancellationToken);
        Task<List<MarketViewModel>> UpdatePostStatus(int rf01_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<MarketViewModel>> DeleteMarket(int rf01_id, int user_id, CancellationToken cancellationToken);
        Task<MarketReqModel> AddMarket(MarketReqModel req, CancellationToken cancellationToken);
        Task<MarketReqModel> UpdateMarket(MarketReqModel req, CancellationToken cancellationToken);
    }
}
