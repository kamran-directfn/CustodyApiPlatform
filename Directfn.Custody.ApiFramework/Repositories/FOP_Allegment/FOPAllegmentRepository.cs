using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_Allegment
{
    public sealed class FOPAllegmentRepository : IFOPAllegmentRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public FOPAllegmentRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }


        public async Task<List<FOPAllegmentViewModel>> GetFopAllegmentAsync(string date, string recAgnt, string delAgnt, int rf48_id, int portfolioId, CancellationToken cancellationToken)
        {
            if (date == "")
            {
                var datecurrent = DateTime.Now.ToShortDateString();
                date = datecurrent;
            }

            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_date", Value = Convert.ToDateTime(date), Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_recAgent", Value = recAgnt, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_delAgnt", Value = delAgnt, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PPRS62_RF48_ID", Value = rf48_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_group_id", Value = portfolioId, Direction = System.Data.ParameterDirection.Input, });

            List<FOPAllegmentViewModel> _lst = await _dbManager.GetStoredProcedureRefCursorAsync<FOPAllegmentViewModel>("PKG_PRS62_FOP_ALLEGMENT.GET_DATA", lstParams, "pview", cancellationToken);

            _lst.ForEach(x => x.PRS62_QUANTITY = x.PRS62_QUANTITY?.Trim(new char[] { ',' }));
            foreach (var data in _lst)
            {
                if (data.PRS62_IS_CANCELLED == 0)
                {
                    data.PRS62_IS_CANCELLED_Des = "No";
                }
                else if (data.PRS62_IS_CANCELLED == 1)
                {
                    data.PRS62_IS_CANCELLED_Des = "Yes";
                }
            }

            return _lst;
        }

        public async Task<List<FOPAllegmentViewModel>> GetFopAllegmentChild(string referenceNo, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_msgRef", Value = referenceNo, Direction = System.Data.ParameterDirection.Input, });

            List<FOPAllegmentViewModel> _lst = await _dbManager.GetStoredProcedureRefCursorAsync<FOPAllegmentViewModel>("PKG_PRS62_FOP_ALLEGMENT.Get_Data_MSGReg", lstParams, "pview", cancellationToken);

            return _lst;
        }

        public async Task<FOPAllegmentViewModel> ExportMT578Msg(int id, CancellationToken cancellationToken)
        {
            List<FOPAllegmentViewModel> _message = new List<FOPAllegmentViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_prs62_id", Value = id, Direction = System.Data.ParameterDirection.Input, });

            _message = await _dbManager.GetStoredProcedureRefCursorAsync<FOPAllegmentViewModel>("PKG_PRS62_FOP_ALLEGMENT.GET_DATA_BY_ID", lstParams, "pview", cancellationToken);

            return _message.FirstOrDefault();
        }
    }
}
