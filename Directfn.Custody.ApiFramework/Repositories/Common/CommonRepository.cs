using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.GroupAccounts;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Common.Enumerations;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public CommonRepository(IOracleDbManagerAsync dbManager, IConfiguration configuration)
        {
            _dbManager = dbManager;
            _configuration = configuration;

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

        public async Task<int> GetBatchID(string ScreenName, int rf48_id, CancellationToken cancellationToken)
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
            _batch.RF42_MEMBER_CODE_ID = rf48_id;


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

        public async Task<List<GroupAccounts>> GetPortfolioAccountsByUser(int portfolioId, int rf48Id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "p_group_id", Value = portfolioId, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "p_rf48_id", Value = rf48Id, Direction = ParameterDirection.Input });

            List<GroupAccounts> data = await _dbManager.GetStoredProcedureRefCursorAsync<GroupAccounts>("PKG_PORTFOLIO_GROUPS.GET_ACC_BY_PORTFOLIO", lstParams, "Pview", cancellationToken);

            return data;
        }

        public List<DropDowns> GetFopTransferTypesDropdown(CancellationToken cancellationToken)
        {
            List<DropDowns> data = Enumerations.GetFopTransferTypes()
               .Select((x, index) => new DropDowns
               {
                   Id = (index + 1).ToString(),
                   text = x.Key,
                   Code = x.Value
               }).ToList();

            return data;
        }

        public List<DropDowns> GetTradeTypes(CancellationToken cancellationToken)
        {
            List<DropDowns> data = new List<DropDowns>();
            data.Add(new DropDowns { Id = "DF", text = "Deliver Free" });
            data.Add(new DropDowns { Id = "RF", text = "Receive Free" });

            return data;
        }

        public List<DropDowns> GetEdaaStatus(CancellationToken cancellationToken)
        {
            List<DropDowns> data = new List<DropDowns>();
            data.Add(new DropDowns { Id = "1", text = "Processed" });
            data.Add(new DropDowns { Id = "2", text = "Un Processed" });

            return data;
        }

        public List<DropDowns> GetAgents(CancellationToken cancellationToken)
        {
            List<DropDowns> data = new List<DropDowns>();

         var agents =   _configuration
            .GetSection("Agents")
            .Get<List<string>>() ?? [];

            foreach ( var agent in agents ) 
                {
                    DropDowns drp = new DropDowns();
                    drp.Id = agent;
                    drp.text = agent;

                    data.Add(drp);
                }

            return data;
        }

        //need to be change converted into extension method
        public string GetReqId()
        {
            string req = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 11);
            req = "ICMS" + req.ToUpper();
            return req;

        }
        //need to be change converted into extension method

        public string EDAA_BIC()
        {
            string edaaBic = _configuration["EDAA_BIC"];
            return edaaBic;
        }

        public string GetMT5BasicHeaderBlock()
        {
            string EDAA_BIC = "";
            string header = "";
            try
            {
                EDAA_BIC = _configuration["EDAA_Header_BIC"];
            }
            catch { }

            header = "{1:F01" + EDAA_BIC + "0000000000}";
            return header;
        }

        public string Get_Header_Block(string messageType, string memberCode)
        {
            string headerBlock = "";
            string senderBIC = "";
            if (!string.IsNullOrEmpty(messageType))
            {
                string key_Sender_Bic = memberCode + "_Sender_BIC";
                string sender_Bic = _configuration[key_Sender_Bic];
                headerBlock = "{2:O" + messageType + "HHMMYYMMDD" + sender_Bic + "0000000000YYMMDDHHMMN}";

                var dt = DateTime.Now.ToString("yyMMdd");
                var tm = DateTime.Now.ToString("hhmm");
                headerBlock = headerBlock.Replace("YYMMDD", dt).Replace("HHMM", tm);
            }
            else
            {
                throw new Exception("Message Type is mandatory.");
            }
            return headerBlock;
        }


        public string GetDateTimeForExportMsgs()
        {
            return DateTime.Now.ToString("yyMMddhhmmss");
        }

    }
}
