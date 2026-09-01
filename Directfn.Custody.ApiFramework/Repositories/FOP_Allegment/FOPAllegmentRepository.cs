using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Repositories.FOP_Allegment
{
    public sealed class FOPAllegmentRepository : IFOPAllegmentRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public FOPAllegmentRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }


        public async Task<List<FOPAllegmentViewModel>> GetFopAllegmentAsync(PaginationRequest<FOPAllegmentFilter> req, int rf48_id, int portfolioId, CancellationToken cancellationToken)
        {
            string filtersObj = JsonSerializer.Serialize(req.filter);

            var parameters = new FOPAllegmentFilter();
            parameters.PageNo = req.PageNo;
            parameters.PageSize = req.PageSize;

            if (req.filter != null && !string.IsNullOrEmpty(req.filter.Trim()) && req.filter != "null")
            {
                FilterClass? filters = JsonSerializer.Deserialize<FilterClass>(filtersObj);
                foreach (var item in filters.filters)
                {
                }
            }

            if (req.sort.Length > 0 && req.sort != "[]")
            {
                var sortValue = JsonSerializer.Deserialize<List<Sort>>(req.sort);
                var field = sortValue.FirstOrDefault().field;
                var dir = sortValue.FirstOrDefault().dir;
                parameters.sorting = field + " " + dir;
            }

            if ( req.Filters.date == "")
            {
                var datecurrent = DateTime.Now.ToShortDateString();
                parameters.date = datecurrent;
            }
            else
            {
                parameters.date = req.Filters.date;
            }
            parameters.recAgnt = req.Filters.recAgnt;
            parameters.delAgnt = req.Filters.delAgnt;

            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_number", Value = parameters.PageNo, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_size", Value = parameters.PageSize, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_date", Value = Convert.ToDateTime(parameters.date), Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_recAgent", Value = parameters.recAgnt, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_delAgnt", Value = parameters.delAgnt, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PPRS62_RF48_ID", Value = rf48_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_group_id", Value = portfolioId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_sorting", Value = parameters.sorting, Direction = System.Data.ParameterDirection.Input, });

            List<FOPAllegmentViewModel> _lst = await _dbManager.GetStoredProcedureRefCursorAsync<FOPAllegmentViewModel>("PKG_PRS62_FOP_ALLEGMENT.GET_DATA_NEW", lstParams, "pview", cancellationToken);

            _lst.ForEach(x => x.PRS62_QUANTITY = x.PRS62_QUANTITY?.Trim(new char[] { ',' }));
            //foreach (var data in _lst)
            //{
            //    if (data.PRS62_IS_CANCELLED == 0)
            //    {
            //        data.PRS62_IS_CANCELLED_Des = "No";
            //    }
            //    else if (data.PRS62_IS_CANCELLED == 1)
            //    {
            //        data.PRS62_IS_CANCELLED_Des = "Yes";
            //    }
            //}

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
