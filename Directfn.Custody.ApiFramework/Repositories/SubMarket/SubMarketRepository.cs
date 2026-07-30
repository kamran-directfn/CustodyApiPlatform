using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.SubMarket
{
    public sealed class SubMarketRepository : ISubMarketRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public SubMarketRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }
       
        public async Task<List<SubMarketViewModel>> GetAllSubMarketsAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<SubMarketViewModel> markets = await _dbManager.GetStoredProcedureRefCursorAsync<SubMarketViewModel>("Pkg_RF16_SUB_MARKETS.GET_DATA", lstParams, "pview", cancellationToken);

            return markets.OrderByDescending(x => x.RF16_CREATED_DATE).ToList(); ;
        }

        public async Task<SubMarketViewModel> GetSubMarketById(int subId, CancellationToken cancellationToken)
        {
            List<SubMarketViewModel> data = await GetAllSubMarketsAsync(cancellationToken);

            SubMarketViewModel sector = data.FirstOrDefault(x => x.RF16_SUB_MARKET_ID == subId);

            return sector;
        }

        public async Task<List<SubMarketViewModel>> UpdatePostStatus(int rf16_sub_market_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF16_SUB_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = rf16_sub_market_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF16_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF16_SUB_MARKETS.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<SubMarketViewModel> data = await GetAllSubMarketsAsync(cancellationToken);
            return data;
        }

        public async Task<List<SubMarketViewModel>> DeleteSubMarket(int rf16_sub_market_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF16_SUB_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = rf16_sub_market_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF16_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF16_SUB_MARKETS.DELETE_DATA", lstParams, cancellationToken);

            List<SubMarketViewModel> data = await GetAllSubMarketsAsync(cancellationToken);
            return data;
        }

        public async Task<SubMarketReqModel> AddSubMarket(SubMarketReqModel _sub, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_DESCRIPTION_SEC", Value = _sub.RF16_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_CURRENCY", Value = _sub.RF16_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_START_TIME", Value = _sub.RF16_START_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_END_TIME", Value = _sub.RF16_END_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_REUTER_CODE", Value = _sub.RF16_REUTER_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_IS_DEFAULT", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_TPLUSN_SELL", Value = _sub.RF16_TPLUSN_SELL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_TPLUSN_BUY", Value = _sub.RF16_TPLUSN_BUY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_MINIMUM_DISCLOSED_QTY", Value = _sub.RF16_MINIMUM_DISCLOSED_QTY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_RF01_MARKET_ID", Value = _sub.RF16_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_MARKET_CODE", Value = _sub.RF16_MARKET_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_RF01_ID", Value = _sub.RF16_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_DESCRIPTION", Value = _sub.RF16_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_EDITED_BY", Value = _sub.RF16_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PERROR", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF16_SUB_MARKETS.ADD_DATA", lstParams);

            _sub.ERROR_MESSAGE = result.GetString("PERROR");

            if (string.IsNullOrEmpty(_sub.ERROR_MESSAGE))
            {
                _sub.RF16_SUB_MARKET_ID = Convert.ToInt32(result.GetString("PKey"));
            }

            return _sub;
        }

        public async Task<SubMarketReqModel> UpdateSubMarket(SubMarketReqModel _sub, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_DESCRIPTION_SEC", Value = _sub.RF16_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_CURRENCY", Value = _sub.RF16_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_START_TIME", Value = _sub.RF16_START_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_END_TIME", Value = _sub.RF16_END_TIME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_REUTER_CODE", Value = _sub.RF16_REUTER_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_IS_DEFAULT", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_TPLUSN_SELL", Value = _sub.RF16_TPLUSN_SELL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_TPLUSN_BUY", Value = _sub.RF16_TPLUSN_BUY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_MINIMUM_DISCLOSED_QTY", Value = _sub.RF16_MINIMUM_DISCLOSED_QTY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_RF01_MARKET_ID", Value = _sub.RF16_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_SUB_MARKET_ID", OracleDbType = OracleDbType.Int32, Value = _sub.RF16_SUB_MARKET_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_MARKET_CODE", Value = _sub.RF16_MARKET_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_RF01_ID", Value = _sub.RF16_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_DESCRIPTION", Value = _sub.RF16_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF16_EDITED_BY", Value = _sub.RF16_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF16_SUB_MARKETS.EDIT_DATA", lstParams);

            _sub.ERROR_MESSAGE = result.GetString("PERROR");

            return _sub;
        }
    }
}
