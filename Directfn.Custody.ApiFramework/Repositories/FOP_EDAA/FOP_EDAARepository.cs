using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using Directfn.Custody.ApiFramework.Common.DTOs.Currency;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
namespace Directfn.Custody.ApiFramework.Repositories.FOP_EDAA
{
    public sealed class FOP_EDAARepository : IFOP_EDAARepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        private readonly ICommonRepository _commonRepository;
        public FOP_EDAARepository(IOracleDbManagerAsync dbManager, ICommonRepository commonRepository)
        {
            _dbManager = dbManager;
            _commonRepository = commonRepository;
        }

        public async Task<List<FOPValidate>> GetAllFOP_EDAA_Async(PaginationRequest req, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            string filtersObj = JsonSerializer.Serialize(req.filter);

            var parameters = new FOPValidate();
            parameters.p_take = req.Take;
            parameters.skip = req.Skip;

            if (req.filter != null && !string.IsNullOrEmpty(req.filter.Trim()) && req.filter != "null")
            {
                FilterClass? filters = JsonSerializer.Deserialize<FilterClass>(filtersObj);
                foreach (var item in filters.filters)
                {
                    if (item.field == "PRS50_TRADE_TYPE")
                    {
                        parameters.PRS50_TRADE_TYPE = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_TRADE_DATE")
                    {
                        parameters.PRS50_TRADE_DATE = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_SETT_DATE")
                    {
                        parameters.PRS50_SETT_DATE = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_ISIN")
                    {
                        parameters.PRS50_ISIN = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_BROKER")
                    {
                        parameters.PRS50_BROKER = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_QUANTITY")
                    {
                        parameters.PRS50_QUANTITY = Convert.ToInt64(item.value);
                    }
                    else if (item.field == "PRS50_UNIQUE_REFERENCE")
                    {
                        parameters.PRS50_UNIQUE_REFERENCE = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_SENDER_CUSTODIAN")
                    {
                        parameters.PRS50_SENDER_CUSTODIAN = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_ACC_SENDER_CUSTODIAN")
                    {
                        parameters.PRS50_ACC_SENDER_CUSTODIAN = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_RECEIVER_CUSTODIAN")
                    {
                        parameters.PRS50_RECEIVER_CUSTODIAN = item.value.Trim().ToString();
                    }
                    else if (item.field == "PRS50_ACC_RECEIVER_CUSTODIAN")
                    {
                        parameters.PRS50_ACC_RECEIVER_CUSTODIAN = item.value.Trim().ToString();
                    }
                }
                
                if (req.sort.Length > 0 && req.sort != "[]")
                {
                    var sortValue = JsonSerializer.Deserialize<List<Sort>>(req.sort);
                    var field = sortValue.FirstOrDefault().field;
                    var dir = sortValue.FirstOrDefault().dir;
                    parameters.sorting = field + " " + dir;
                }

                parameters.PRS50_TRADE_DATE = req.TradeDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                if (parameters.PRS50_TRADE_DATE == "00010101")
                {
                    parameters.PRS50_TRADE_DATE = null;
                }
                parameters.PRS50_SETT_DATE = req.SettlementDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                if (parameters.PRS50_SETT_DATE == "00010101")
                {
                    parameters.PRS50_SETT_DATE = null;
                }
                parameters.PRS50_TRADE_TYPE = req.TradeType;
                parameters.PRS50_UNIQUE_REFERENCE = req.UniqueReference;
                parameters.PRS50_TRANSFER_TYPE = req.TransferType;
            }

            List<FOPValidate> _lst = new List<FOPValidate>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_skip", Value = parameters.skip, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_take", Value = parameters.p_take, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_TRADE_TYPE", Value = parameters.PRS50_TRADE_TYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_TRADE_DATE", Value = parameters.PRS50_TRADE_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_SETT_DATE", Value = parameters.PRS50_SETT_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_ISIN", Value = parameters.PRS50_ISIN, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_BROKER", Value = parameters.PRS50_BROKER, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_QUANTITY", Value = parameters.PRS50_QUANTITY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_UNIQUE_REFERENCE", Value = parameters.PRS50_UNIQUE_REFERENCE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_SENDER_CUSTODIAN", Value = parameters.PRS50_SENDER_CUSTODIAN, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_ACC_SENDER_CUSTODIAN", Value = parameters.PRS50_ACC_SENDER_CUSTODIAN, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_RECEIVER_CUSTODIAN", Value = parameters.PRS50_RECEIVER_CUSTODIAN, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_ACC_RECEIVER_CUSTODIAN", Value = parameters.PRS50_ACC_RECEIVER_CUSTODIAN, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_sorting", Value = parameters.sorting, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_rf48_id", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRS50_TRANSFER_TYPE", Value = parameters.PRS50_TRANSFER_TYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "IsSent", Value = req.IsSent, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_group_id", Value = groupId, Direction = System.Data.ParameterDirection.Input, });
            
            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<FOPValidate>("Pkg_PRS50_FOP.Get_Data_EDAA", lstParams, "pview", cancellationToken);
            
            return _lst;
        }
    }
}
