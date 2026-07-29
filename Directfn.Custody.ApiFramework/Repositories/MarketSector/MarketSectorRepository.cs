using Directfn.Custody.ApiFramework.Common.DTOs.Market;
using Directfn.Custody.ApiFramework.Common.DTOs.MarketSector;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.MarketSector
{
    public sealed class MarketSectorRepository : IMarketSectorRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public MarketSectorRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<MarketSectorViewModel>> GetAllMarketSectorsAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<MarketSectorViewModel> markets = await _dbManager.GetStoredProcedureRefCursorAsync<MarketSectorViewModel>("Pkg_RF40_MARKET_SECTORS.GET_DATA", lstParams, "pview", cancellationToken);

            return markets.OrderByDescending(x => x.RF40_CREATED_DATE).ToList(); ;
        }

        public async Task<MarketSectorViewModel> GetSectorById(int sectorId, CancellationToken cancellationToken)
        {
            List<MarketSectorViewModel> data = await GetAllMarketSectorsAsync(cancellationToken);

            MarketSectorViewModel sector = data.FirstOrDefault(x => x.RF40_ID == sectorId);

            return sector;
        }
        public async Task<List<MarketSectorViewModel>> UpdatePostStatus(int rf40_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF40_ID", OracleDbType = OracleDbType.Int32, Value = rf40_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF40_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF40_MARKET_SECTORS.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<MarketSectorViewModel> data = await GetAllMarketSectorsAsync(cancellationToken);
            return data;
        }
        public async Task<List<MarketSectorViewModel>> DeleteSector(int rf40_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF40_ID", OracleDbType = OracleDbType.Int32, Value = rf40_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF40_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF40_MARKET_SECTORS.Delete_Data", lstParams, cancellationToken);

            List<MarketSectorViewModel> data = await GetAllMarketSectorsAsync(cancellationToken);
            return data;
        }
        public async Task<MarketSectorReqModel> AddMarketSector(MarketSectorReqModel _sector, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_NAME_SEC", Value = _sector.RF40_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_RF01_ID", Value = _sector.RF40_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_CODE", Value = _sector.RF40_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_NAME", Value = _sector.RF40_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_Edited_by", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_Created_by", Value = _sector.RF40_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF40_MARKET_SECTORS.ADD_DATA", lstParams);

            _sector.RF40_ID = Convert.ToInt32(result.GetString("PKey"));

            return _sector;
        }
        public async Task<MarketSectorReqModel> UpdateMarketSector(MarketSectorReqModel _sector, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_NAME_SEC", Value = _sector.RF40_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_RF01_ID", Value = _sector.RF40_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_CODE", Value = _sector.RF40_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_ID", OracleDbType = OracleDbType.Int32, Value = _sector.RF40_ID, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_NAME", Value = _sector.RF40_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF40_Edited_by", Value = _sector.RF40_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF40_MARKET_SECTORS.EDIT_DATA", lstParams);

            return _sector;
        }

      
    }
}
