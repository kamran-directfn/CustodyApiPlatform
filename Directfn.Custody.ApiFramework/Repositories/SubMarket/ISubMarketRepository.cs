using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.SubMarket
{
    public interface ISubMarketRepository
    {
        Task<List<SubMarketViewModel>> GetAllSubMarketsAsync(CancellationToken cancellationToken);
        Task<SubMarketViewModel> GetSubMarketById(int subId, CancellationToken cancellationToken);
        Task<List<SubMarketViewModel>> UpdatePostStatus(int rf16_sub_market_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<SubMarketViewModel>> DeleteSubMarket(int rf16_sub_market_id, int user_id, CancellationToken cancellationToken);
        Task<SubMarketReqModel> AddSubMarket(SubMarketReqModel _sub, CancellationToken cancellationToken);
        Task<SubMarketReqModel> UpdateSubMarket(SubMarketReqModel _sub, CancellationToken cancellationToken);
    }
}
