using Directfn.Custody.ApiFramework.Common.DTOs.Market;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Market
{
    public sealed class MarketRepository : IMarketRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public MarketRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }
        public async Task<List<MarketViewModel>> GetAllMarketsAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<MarketViewModel> markets = await _dbManager.GetStoredProcedureRefCursorAsync<MarketViewModel>("Pkg_RF01_MARKETS.GET_DATA", lstParams, "pview", cancellationToken);

            return markets.OrderByDescending(x => x.RF01_MARKET_ID).ToList(); ;
        }

        public async Task<MarketViewModel> GetMarketById(int marketId, CancellationToken cancellationToken)
        {
            List<MarketViewModel> data = await GetAllMarketsAsync(cancellationToken);

            MarketViewModel market = data.FirstOrDefault(x => x.RF01_MARKET_ID == marketId);

            return market;
        }

        public async Task<List<MarketViewModel>> UpdatePostStatus(int rf01_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF01_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = rf01_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF01_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF01_MARKETS.Update_Post_Status", lstParams, cancellationToken);

            List<MarketViewModel> data = await GetAllMarketsAsync(cancellationToken);
            return data;
        }

        public async Task<List<MarketViewModel>> DeleteMarket(int rf01_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF01_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = rf01_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF01_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF01_MARKETS.Delete_Data", lstParams, cancellationToken);

            List<MarketViewModel> data = await GetAllMarketsAsync(cancellationToken);
            return data;
        }
      
        public async Task<MarketReqModel> AddMarket(MarketReqModel market, CancellationToken cancellationToken)
        {
            if (market.Rf01_Weekend_Arr != null)
            {
                market.RF01_WEEKEND = string.Join(",", market.Rf01_Weekend_Arr);
            }

            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_REUTER_CODE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_START_TIME", Value = market.RF01_START_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_END_TIME", Value = market.RF01_END_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_WEEKEND", Value = market.RF01_WEEKEND, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_LANGUAGE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_CURRENCY", Value = market.RF01_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_CODE", Value = market.RF01_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_DESCRIPTION", Value = market.RF01_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_DESCRIPTION_SEC", Value = market.RF01_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_TYPE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_EDITED_BY", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_CREATED_BY", Value = market.RF01_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "P_Error", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF01_MARKETS.Add_Data", lstParams);

            market.RF01_MARKET_ID = Convert.ToInt32(result.GetString("PKey"));

            return market;
        }

        public async Task<MarketReqModel> UpdateMarket(MarketReqModel market, CancellationToken cancellationToken)
        {
            if (market.Rf01_Weekend_Arr != null)
            {
                market.RF01_WEEKEND = string.Join(",", market.Rf01_Weekend_Arr);
            }

            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_REUTER_CODE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_START_TIME", Value = market.RF01_START_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_END_TIME", Value = market.RF01_END_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_WEEKEND", Value = market.RF01_WEEKEND, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_LANGUAGE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_CURRENCY", Value = market.RF01_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = market.RF01_MARKET_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_CODE", Value = market.RF01_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_DESCRIPTION", Value = market.RF01_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_DESCRIPTION_SEC", Value = market.RF01_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_TYPE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF01_EDITED_BY", Value = market.RF01_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF01_MARKETS.Edit_Data", lstParams);

            return market;
        }
        
    }
}
