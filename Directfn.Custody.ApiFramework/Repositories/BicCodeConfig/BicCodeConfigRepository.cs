using Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig;
using Directfn.Custody.ApiFramework.Common.DTOs.SubMarket;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.BicCodeConfig
{
    public sealed class BicCodeConfigRepository : IBicCodeConfigRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public BicCodeConfigRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }
       

        public async Task<List<BicCodeConfigViewModel>> GetAllBicCodeConfigsAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<BicCodeConfigViewModel> _bicCode = await _dbManager.GetStoredProcedureRefCursorAsync<BicCodeConfigViewModel>("Pkg_RF84_BIC_CODE_CONFIG.GET_DATA", lstParams, "pview", cancellationToken);

            return _bicCode.OrderByDescending(x => x.RF84_ID).ToList();
        }

        public async Task<BicCodeConfigViewModel> GetBicCodeConfigById(int bicCodeId, CancellationToken cancellationToken)
        {
            List<BicCodeConfigViewModel> data = await GetAllBicCodeConfigsAsync(cancellationToken);

            BicCodeConfigViewModel _bicCode = data.FirstOrDefault(x => x.RF84_ID == bicCodeId);

            return _bicCode;
        }

        public async Task<List<BicCodeConfigViewModel>> UpdatePostStatus(int rf84_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF84_ID", OracleDbType = OracleDbType.Int32, Value = rf84_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF84_IS_POSTED", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF84_BIC_CODE_CONFIG.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<BicCodeConfigViewModel> data = await GetAllBicCodeConfigsAsync(cancellationToken);
            return data;
        }

        public async Task<List<BicCodeConfigViewModel>> DeleteBicCodeConfig(int rf84_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_ID", Value = rf84_id, Direction = System.Data.ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_Edited_by", Value = user_id, Direction = System.Data.ParameterDirection.Input });
           
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF84_BIC_CODE_CONFIG.DELETE_DATA", lstParams, cancellationToken);
           
            List<BicCodeConfigViewModel> data = await GetAllBicCodeConfigsAsync(cancellationToken);
            return data;
        }

        public async Task<BicCodeConfigReqModel> AddBicCodeConfig(BicCodeConfigReqModel _bicCode, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_NAME", Value = _bicCode.RF84_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_NAME_SEC", Value = _bicCode.RF84_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_BIC_CODE", Value = _bicCode.RF84_BIC_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_CREATED_BY", Value = _bicCode.RF84_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_EDITED_By", Value = null, Direction = System.Data.ParameterDirection.Input, });
           
            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF84_BIC_CODE_CONFIG.ADD_DATA", lstParams);

            _bicCode.RF84_ID = Convert.ToInt32(result.GetString("PKey"));
            
            return _bicCode;
        }
        public async Task<BicCodeConfigReqModel> UpdateBicCodeConfig(BicCodeConfigReqModel _bicCode, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_ID", Value = _bicCode.RF84_ID, Direction = System.Data.ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_NAME", Value = _bicCode.RF84_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_NAME_SEC", Value = _bicCode.RF84_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_BIC_CODE", Value = _bicCode.RF84_BIC_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF84_EDITED_By", Value = _bicCode.RF84_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF84_BIC_CODE_CONFIG.EDIT_DATA", lstParams);

            return _bicCode;
        }
    }
}