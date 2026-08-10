using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Directfn.Custody.ApiFramework.Repositories.Common
{
    public sealed class CommonRepository : ICommonRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public CommonRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<List<DropDowns>> GetRoles(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Roles_DropDown_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<DropDowns>> GetMemberCode(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_MemberCode_DropDown_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<PortfoliosByMembers>> GetGroupsByMember(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<PortfoliosByMembers>("pkg_portfolio_groups.get_groups_by_member", lstParams, "Pview", cancellationToken);

            return data;
        }

        public DataSet UploadFileDataSet(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            IExcelDataReader reader = Path.GetExtension(file.FileName).ToLower() switch
            {
                ".xls" => ExcelReaderFactory.CreateBinaryReader(stream),
                ".xlsx" => ExcelReaderFactory.CreateOpenXmlReader(stream),
                ".csv" => ExcelReaderFactory.CreateCsvReader(stream),
                _ => throw new NotSupportedException("Unsupported file format.")
            };

            return reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });
        }

        public async Task<int> GetBatchID(string ScreenName, CancellationToken cancellationToken)
        {
            int batchid = 0;
            BatchExecute _batch = new BatchExecute();
            _batch.RF42_TYPE = ScreenName;
            _batch.RF42_CREATED_BY = 1;
            _batch.RF42_CREATED_DATE = DateTime.Now;
            _batch.RF42_DESCRIPTION = ScreenName;
            _batch.RF42_DESCRIPTION_SEC = ScreenName;
            _batch.RF42_KIND = ScreenName;
            _batch.RF42_STATUS = 1;
            _batch.RF42_TYPE = ScreenName;
            _batch.RF42_MEMBER_CODE_ID = 1;// User.MemberCodeID; need to change by shahzaib


            var parameters = new List<OracleParameter>();

            if (_batch.RF42_ID > 0)
                parameters.Add(new OracleParameter { ParameterName = "PRF42_ID", Value = _batch.RF42_ID, Direction = ParameterDirection.Input });
            else
                parameters.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });

            parameters.Add(new OracleParameter { ParameterName = "PRF42_DESCRIPTION", Value = _batch.RF42_DESCRIPTION, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_DESCRIPTION_SEC", Value = _batch.RF42_DESCRIPTION_SEC, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_TYPE", Value = _batch.RF42_TYPE, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_STATUS", Value = 1, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_KIND", Value = _batch.RF42_KIND, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_IP", Value = _batch.RF42_IP, Direction = ParameterDirection.Input });
            parameters.Add(new OracleParameter { ParameterName = "PRF42_Edited_by", Value = _batch.RF42_MODIFIED_BY, Direction = ParameterDirection.Input });
            //parameters.Add(new OracleParameter { ParameterName = "PRF42_TIME", Value = _batch.RF42_TIME, Direction = ParameterDirection.Input });

            if (_batch.RF42_ID > 0)
            {
                await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF42_BATCH.Edit_Data", parameters, cancellationToken);
            }
            else
            {
                parameters.Add(new OracleParameter { ParameterName = "PRF42_Member_Code_Id", Value = _batch.RF42_MEMBER_CODE_ID, Direction = ParameterDirection.Input });
                
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF42_BATCH.Add_Data", parameters, cancellationToken);
                batchid = int.Parse(result.GetString("PKey"));
            }

            return batchid;

        }

        public async Task<List<DropDowns>> GetBanks(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Bank_Dropdown", lstParams, "Pview", cancellationToken);

            return data;

        }

        public async Task<List<DropDowns>> GetMarkets(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Market_Dropdown", lstParams, "Pview", cancellationToken);

            return data;

        }

        public async Task<List<DropDowns>> GetCurrencies(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Currency_Dropdown", lstParams, "Pview", cancellationToken);

            return data;

        }

        public async Task<List<DropDowns>> GetCountries(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Countries_New", lstParams, "Pview", cancellationToken);

            return data;

        }

        public async Task<List<DropDowns>> GetEconomicSector(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Economic_Sector_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<DropDowns>> GetMarketSector(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Market_Sector_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<DropDowns>> GetSubMarket(int marketId, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "P_market", Value = marketId, Direction = ParameterDirection.Input });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_SubMarket_Dropdown", lstParams, "Pview", cancellationToken);

            return data;
        }
    }
}
