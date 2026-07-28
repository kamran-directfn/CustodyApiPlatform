using Directfn.Custody.ApiFramework.Common.DTOs.Cities;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Cities
{
    public sealed class CitiesRepository : ICitiesRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public CitiesRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }
        
        public async Task<List<CitiesViewModel>> GetAllCitiesAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<CitiesViewModel> _cities = await _dbManager.GetStoredProcedureRefCursorAsync<CitiesViewModel>("PKG_RF10_CITIES.GET_DATA", lstParams, "pview", cancellationToken);

            return _cities.OrderByDescending(x => x.RF10_CITY_ID).ToList();
        }

        public async Task<CitiesViewModel> GetCityById(int cityId, CancellationToken cancellationToken)
        {
            List<CitiesViewModel> data = await GetAllCitiesAsync(cancellationToken);

            CitiesViewModel _city = data.FirstOrDefault(x => x.RF10_CITY_ID == cityId);

            return _city;
        }

        public async Task<List<CitiesViewModel>> UpdatePostStatus(int rf10_city_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF10_CITY_ID", OracleDbType = OracleDbType.Int32, Value = rf10_city_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF10_IS_POSTED", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("PKG_RF10_CITIES.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<CitiesViewModel> data = await GetAllCitiesAsync(cancellationToken);
            return data;
        }

        public async Task<List<CitiesViewModel>> DeleteCity(int rf10_city_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF10_CITY_ID", OracleDbType = OracleDbType.Int32, Value = rf10_city_id, Direction = ParameterDirection.Input });
           
            await _dbManager.ExecuteStoredProcedureAsync("PKG_RF10_CITIES.DELETE_DATA", lstParams, cancellationToken);

            List<CitiesViewModel> data = await GetAllCitiesAsync(cancellationToken);
            return data;
        }

        public async Task<CitiesReqModel> SaveCity(CitiesReqModel _cities, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_DESCRIPTION", Value = _cities.RF10_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_DESCRIPTION_AR", Value = _cities.RF10_DESCRIPTION_AR, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_COUNTRY_ID", Value = _cities.RF10_COUNTRY_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_Edited_By", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_Created_By", Value = _cities.RF10_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF10_CITIES.ADD_DATA", lstParams);

            _cities.RF10_CITY_ID = Convert.ToInt32(result.GetString("PKey"));

            return _cities;
        }

        public async Task<CitiesReqModel> UpdateCity(CitiesReqModel _cities, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_CITY_ID", OracleDbType = OracleDbType.Int32, Value = _cities.RF10_CITY_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_DESCRIPTION", Value = _cities.RF10_DESCRIPTION, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_DESCRIPTION_AR", Value = _cities.RF10_DESCRIPTION_AR, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_COUNTRY_ID", Value = _cities.RF10_COUNTRY_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF10_EDITED_BY", Value = _cities.RF10_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF10_CITIES.EDIT_DATA", lstParams);

            return _cities;
        }
    }
}