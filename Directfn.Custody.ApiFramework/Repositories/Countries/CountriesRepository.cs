using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Countries
{
    public sealed class CountriesRepository : ICountriesRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public CountriesRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<CountriesViewModel>> GetAllCountriesAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<CountriesViewModel> _countries = await _dbManager.GetStoredProcedureRefCursorAsync<CountriesViewModel>("Pkg_RF09_COUNTRY.GET_DATA", lstParams, "pview", cancellationToken);

            return _countries.OrderByDescending(x => x.RF09_COUNTRY_ID).ToList(); 
        }

        public async Task<CountriesViewModel> GetCountryById(int countryId, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF09_COUNTRY_ID", OracleDbType = OracleDbType.Int32, Value = countryId, Direction = ParameterDirection.Input });

            List<CountriesViewModel> _country = await _dbManager.GetStoredProcedureRefCursorAsync<CountriesViewModel>("Pkg_RF09_COUNTRY.GET_DATA_BY_ID", lstParams, "pview", cancellationToken);

            return _country.FirstOrDefault();
        }

        public async Task<List<CountriesViewModel>> UpdatePostStatus(int rf09_country_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF09_COUNTRY_ID", OracleDbType = OracleDbType.Int32, Value = rf09_country_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF09_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF09_COUNTRY.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<CountriesViewModel> data = await GetAllCountriesAsync(cancellationToken);
            return data;
        }

        public async Task<List<CountriesViewModel>> DeleteCountry(int rf09_country_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF09_COUNTRY_ID", OracleDbType = OracleDbType.Int32, Value = rf09_country_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF09_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF09_COUNTRY.DELETE_DATA", lstParams, cancellationToken);

            List<CountriesViewModel> data = await GetAllCountriesAsync(cancellationToken);
            return data;
        }

        public async Task<CountriesReqModel> SaveCountry(CountriesReqModel _countries, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_CMS_COUNTRY_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IS_ACTIVE", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IP_ADDRESS", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_COUNTRY_CODE", Value = _countries.RF09_COUNTRY_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_ARABIC_NAME", Value = _countries.RF09_ARABIC_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_ID", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_CHANGED_BY", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_CHANGED_DATE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_NATIONALITY_CATEGORY", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IS_GCC", Value = _countries.RF09_IS_GCC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_VISIBLE", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_COUNTRY_NAME", Value = _countries.RF09_COUNTRY_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_Edited_by", Value = _countries.RF09_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_Created_by", Value = _countries.RF09_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PERROR", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF09_COUNTRY.ADD_DATA", lstParams);

            _countries.ERROR_MESSAGE = result.GetString("PERROR");

            if(string.IsNullOrEmpty(_countries.ERROR_MESSAGE))
            {
                _countries.RF09_COUNTRY_ID = Convert.ToInt32(result.GetString("PKey"));
            }

            return _countries;
        }

        public async Task<CountriesReqModel> UpdateCountry(CountriesReqModel _countries, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_CMS_COUNTRY_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IS_ACTIVE", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IP_ADDRESS", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_COUNTRY_CODE", Value = _countries.RF09_COUNTRY_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_ARABIC_NAME", Value = _countries.RF09_ARABIC_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_ID", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_CHANGED_BY", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_STATUS_CHANGED_DATE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_NATIONALITY_CATEGORY", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_IS_GCC", Value = _countries.RF09_IS_GCC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_VISIBLE", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_COUNTRY_ID", Value = _countries.RF09_COUNTRY_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_COUNTRY_NAME", Value = _countries.RF09_COUNTRY_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF09_Edited_by", Value = _countries.RF09_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PERROR", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF09_COUNTRY.EDIT_DATA", lstParams);

            _countries.ERROR_MESSAGE = result.GetString("PERROR");
            return _countries;
        }

    }
}
