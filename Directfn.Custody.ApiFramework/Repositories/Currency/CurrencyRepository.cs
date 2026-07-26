using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Currency;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Currency
{
    public sealed class CurrencyRepository : ICurrencyRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public CurrencyRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<CurrencyViewModel>> GetAllCurrenciesAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<CurrencyViewModel> currencires = await _dbManager.GetStoredProcedureRefCursorAsync<CurrencyViewModel>("Pkg_RF08_CURRENCY.GET_DATA", lstParams, "pview", cancellationToken);

            return currencires;
        }

        public async Task<CurrencyViewModel> GetCurrencyById(int currencyId, CancellationToken cancellationToken)
        {

            List<CurrencyViewModel> data = await GetAllCurrenciesAsync(cancellationToken);

            CurrencyViewModel currency = data.FirstOrDefault(x => x.RF08_ID == currencyId);

            return currency;
        }

        public async Task<List<CurrencyViewModel>> UpdatePostStatus(int rf08_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF08_ID", OracleDbType = OracleDbType.Int32, Value = rf08_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF08_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF08_CURRENCY.Update_Post_Status", lstParams, cancellationToken);

            List<CurrencyViewModel> data = await GetAllCurrenciesAsync(cancellationToken);
            return data;
        }

        public async Task<List<CurrencyViewModel>> DeleteCurrency(int rf08_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF08_ID", OracleDbType = OracleDbType.Int32, Value = rf08_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF08_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF08_CURRENCY.Delete_Data", lstParams, cancellationToken);

            List<CurrencyViewModel> data = await GetAllCurrenciesAsync(cancellationToken);
            return data;
        }

        public async Task<CurrencyReqModel> AddCurrency(CurrencyReqModel currency, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_DESCRIPTION_SEC", Value = currency.RF08_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_SELL_RATE", Value = currency.RF08_SELL_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_BUY_RATE", Value = currency.RF08_BUY_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_AVG_RATE", Value = currency.RF08_AVG_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_FIX_RATE", Value = currency.RF08_FIX_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_CODE", Value = currency.RF08_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_DESCRIPTION", Value = currency.RF08_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_EDITED_BY", Value = currency.RF08_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF08_CURRENCY.Add_Data", lstParams);

            currency.RF08_ID = Convert.ToInt32(result.GetString("PKey"));

            return currency;
        }

        public async Task<CurrencyReqModel> UpdateCurrency(CurrencyReqModel currency, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_DESCRIPTION_SEC", Value = currency.RF08_DESCRIPTION_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_SELL_RATE", Value = currency.RF08_SELL_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_BUY_RATE", Value = currency.RF08_BUY_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_AVG_RATE", Value = currency.RF08_AVG_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_FIX_RATE", Value = currency.RF08_FIX_RATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_ID", OracleDbType = OracleDbType.Int32, Value = currency.RF08_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_CODE", Value = currency.RF08_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_DESCRIPTION", Value = currency.RF08_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF08_EDITED_BY", Value = currency.RF08_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF08_CURRENCY.Edit_Data", lstParams);

            return currency;
        }
      
    }
}
