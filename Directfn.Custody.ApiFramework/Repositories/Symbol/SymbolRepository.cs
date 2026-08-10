using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using Directfn.Custody.ApiFramework.Common.DTOs.Symbol;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Symbol
{
    public sealed class SymbolRepository : ISymbolRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public SymbolRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<SymbolViewModel>> GetAllSymbolsAsync(CancellationToken cancellationToken)
        {

            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<SymbolViewModel> _symbols = await _dbManager.GetStoredProcedureRefCursorAsync<SymbolViewModel>("Pkg_RF02_SYMBOLS.Get_Data", lstParams, "pview", cancellationToken);

            return _symbols;
        }

        public async Task<SymbolViewModel> GetSymbolById(int symbolId, CancellationToken cancellationToken)
        {
            List<SymbolViewModel> data = await GetAllSymbolsAsync(cancellationToken);

            SymbolViewModel _symbol = data.FirstOrDefault(x => x.RF02_SYMBOL_ID == symbolId);

            return _symbol;
        }

        public async Task<List<SymbolViewModel>> UpdatePostStatus(int rf02_symbol_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF02_SYMBOL_ID", OracleDbType = OracleDbType.Int32, Value = rf02_symbol_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF02_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("PKG_RF02_SYMBOLS.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<SymbolViewModel> _symbols = await GetAllSymbolsAsync(cancellationToken);

            return _symbols;
        }

        public async Task<List<SymbolViewModel>> DeleteSymbol(int rf02_symbol_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF02_SYMBOL_ID", OracleDbType = OracleDbType.Int32, Value = rf02_symbol_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF02_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF02_SYMBOLS.Delete_Data", lstParams, cancellationToken);

            List<SymbolViewModel> _symbols = await GetAllSymbolsAsync(cancellationToken);

            return _symbols;
        }

        public async Task<SymbolReqModel> AddSymbol(SymbolReqModel _symbol, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_FACE_VALUE", Value = _symbol.RF02_FACE_VALUE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ROUTER_CODE", Value = _symbol.RF02_ROUTER_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CUSIP_NUMBER", Value = _symbol.RF02_CUSIP_NUMBER, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LOT_SIZE", Value = _symbol.RF02_LOT_SIZE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF45_ID", Value = _symbol.RF02_RF45_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF40_ID", Value = _symbol.RF02_RF40_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF16_ID", Value = _symbol.RF02_RF16_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISSUE_SIZE", Value = _symbol.RF02_ISSUE_SIZE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISSUE_DATE", Value = _symbol.RF02_ISSUE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BASIS", Value = _symbol.RF02_BASIS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_STOPLOSS_PERCENT", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LANGUAGE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CAPITAL", Value = _symbol.RF02_CAPITAL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LISTED_PRICE", Value = _symbol.RF02_LISTED_PRICE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF43_ID", Value = _symbol.RF02_RF43_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_START_DATE", Value = _symbol.RF02_ISSUE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF01_ID", Value = _symbol.RF02_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL_DESC", Value = _symbol.RF02_SYMBOL_DESC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL_DESC_SEC", Value = _symbol.RF02_SYMBOL_DESC_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHORT_DESC", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHORT_DESC_SEC", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL", Value = _symbol.RF02_SYMBOL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CURRENCY", Value = _symbol.RF02_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHARIA_COMPLIENT", Value = _symbol.RF02_SHARIA_COMPLIENT, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISINCODE", Value = _symbol.RF02_ISINCODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EXPIRE_DATE", Value = _symbol.RF02_EXPIRE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_PRICE_RATIO", Value = _symbol.RF02_PRICE_RATIO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BENCHMARK", Value = _symbol.RF02_BENCHMARK, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BLOOMBERG_CODE", Value = _symbol.RF02_BLOOMBERG_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EXEC_BROKER_SID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CEDEL_NO", Value = _symbol.RF02_CEDEL_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SEDOL_NO", Value = _symbol.RF02_SEDOL_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EDITED_BY", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CREATED_BY", Value = _symbol.RF02_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_GLOBAL_CODE", Value = _symbol.RF02_GLOBAL_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PERROR", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF02_SYMBOLS.ADD_DATA", lstParams);

            _symbol.ERROR_MESSAGE = result.GetString("PERROR");

            if (string.IsNullOrEmpty(_symbol.ERROR_MESSAGE))
            {
                _symbol.RF02_SYMBOL_ID = Convert.ToInt32(result.GetString("PKey"));
            }

            return _symbol;
        }

        public async Task<SymbolReqModel> UpdateSymbol(SymbolReqModel _symbol, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_FACE_VALUE", Value = _symbol.RF02_FACE_VALUE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ROUTER_CODE", Value = _symbol.RF02_ROUTER_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CUSIP_NUMBER", Value = _symbol.RF02_CUSIP_NUMBER, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LOT_SIZE", Value = _symbol.RF02_LOT_SIZE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF45_ID", Value = _symbol.RF02_RF45_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF40_ID", Value = _symbol.RF02_RF40_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF16_ID", Value = _symbol.RF02_RF16_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISSUE_SIZE", Value = _symbol.RF02_ISSUE_SIZE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISSUE_DATE", Value = _symbol.RF02_ISSUE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BASIS", Value = _symbol.RF02_BASIS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_STOPLOSS_PERCENT", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LANGUAGE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CAPITAL", Value = _symbol.RF02_CAPITAL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_LISTED_PRICE", Value = _symbol.RF02_LISTED_PRICE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF43_ID", Value = _symbol.RF02_RF43_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_START_DATE", Value = _symbol.RF02_ISSUE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL_ID", OracleDbType = OracleDbType.Int32, Value = _symbol.RF02_SYMBOL_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_RF01_ID", Value = _symbol.RF02_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL_DESC", Value = _symbol.RF02_SYMBOL_DESC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL_DESC_SEC", Value = _symbol.RF02_SYMBOL_DESC_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHORT_DESC", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHORT_DESC_SEC", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SYMBOL", Value = _symbol.RF02_SYMBOL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CURRENCY", Value = _symbol.RF02_CURRENCY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SHARIA_COMPLIENT", Value = _symbol.RF02_SHARIA_COMPLIENT, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_ISINCODE", Value = _symbol.RF02_ISINCODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EXPIRE_DATE", Value = _symbol.RF02_EXPIRE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_PRICE_RATIO", Value = _symbol.RF02_PRICE_RATIO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BENCHMARK", Value = _symbol.RF02_BENCHMARK, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_BLOOMBERG_CODE", Value = _symbol.RF02_BLOOMBERG_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EXEC_BROKER_SID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_CEDEL_NO", Value = _symbol.RF02_CEDEL_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_SEDOL_NO", Value = _symbol.RF02_SEDOL_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_EDITED_BY", Value = _symbol.RF02_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF02_GLOBAL_CODE", Value = _symbol.RF02_GLOBAL_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PERROR", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF02_SYMBOLS.EDIT_DATA", lstParams);

            _symbol.ERROR_MESSAGE = result.GetString("PERROR");

            return _symbol;
        }
    }
}
