using Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.HeadRoomConfig
{
    public sealed class HeadRoomConfigRepository : IHeadRoomConfigRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public HeadRoomConfigRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }
        
        public async Task<List<HeadRoomConfigViewModel>> GetHeadRoomConfigsAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<HeadRoomConfigViewModel> _headRoom = await _dbManager.GetStoredProcedureRefCursorAsync<HeadRoomConfigViewModel>("PKG_HEADROOMCONFIG.GET_HEADROOM_CONFIG_DATA", lstParams, "pview", cancellationToken);

            return _headRoom;
        }

        public async Task<HeadRoomConfigReqModel> UpdateHeadRoomConfig(HeadRoomConfigReqModel _headRoom, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "cap", Value = _headRoom.SETTLEMENT_CAP, Direction = System.Data.ParameterDirection.Input });
            lstParams.Add(new OracleParameter() { ParameterName = "update_limit", Value = _headRoom.LIMIT_UPDATE_EDAA, Direction = System.Data.ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("PKG_HEADROOMCONFIG.UPDATE_HEADROOM_CONFIG", lstParams, cancellationToken);

            return _headRoom;
        }
    }
}
