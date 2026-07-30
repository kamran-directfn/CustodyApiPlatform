using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using ExcelDataReader;

using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.PortfolioGroup
{
    public sealed class PortfolioGroupRepository : IPortfolioGroupRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public PortfolioGroupRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<PortfolioGroupViewModel>> GetPortfolio(int rf48_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "p_rf48_id", OracleDbType = OracleDbType.Varchar2, Value = rf48_id, Direction = ParameterDirection.Input });

            var portfolio = await _dbManager.GetStoredProcedureRefCursorAsync<PortfolioGroupViewModel>("pkg_portfolio_groups.Get_Portfolios", lstParams, "pview", cancellationToken);

            return portfolio;
        }

        public async Task<PortfolioGroupById> GetPortfolioByID(int um14_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "pview2", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "p_group_id", OracleDbType = OracleDbType.Varchar2, Value = um14_id, Direction = ParameterDirection.Input });

            var _result = await _dbManager.GetStoredProcedureDataSetResultAsync("pkg_portfolio_groups.Get_By_ID", lstParams, cancellationToken);

            PortfolioGroupById portfolioGroup = new PortfolioGroupById();

            if (_result != null && _result.Tables != null && _result.Tables.Count > 0)
            {
                var basicDetails = _result.Tables[0];
                var portfolioDetails = _result.Tables[1];

                if (basicDetails != null && basicDetails.Rows != null && basicDetails.Rows.Count == 1)
                {
                    portfolioGroup.Group = new PortfolioGroupViewModel();
                    portfolioGroup.Group.um14_id = Convert.ToInt32(basicDetails.Rows[0]["UM14_ID"]);
                    portfolioGroup.Group.um14_group_name = basicDetails.Rows[0]["UM14_GROUP_NAME"].ToString();
                    portfolioGroup.Group.um14_group_description = basicDetails.Rows[0]["UM14_GROUP_DESCRIPTION"].ToString();
                    portfolioGroup.Group.um14_rf48_id = Convert.ToInt32(basicDetails.Rows[0]["UM14_RF48_ID"]);
                    portfolioGroup.Group.um14_created_by = Convert.ToInt32(basicDetails.Rows[0]["UM14_CREATED_BY"]);

                    if (portfolioDetails.Rows.Count > 0)
                    {
                        portfolioGroup.Group_Details = new List<PortfolioGroup_Details>();

                        for (int i = 0; i < portfolioDetails.Rows.Count; i++)
                        {
                            PortfolioGroup_Details detail = new PortfolioGroup_Details();
                            detail.UM15_ID = Convert.ToInt32(portfolioDetails.Rows[i]["UM15_ID"]);
                            detail.UM15_SECURITY_ACCOUNT = portfolioDetails.Rows[i]["UM15_SECURITY_ACCOUNT"].ToString();
                            detail.UM15_CREATED_BY = Convert.ToInt32(portfolioDetails.Rows[i]["UM15_CREATED_BY"]);
                            detail.UM15_CREATED_DATE = Convert.ToDateTime(portfolioDetails.Rows[i]["UM15_CREATED_DATE"]);

                            portfolioGroup.Group_Details.Add(detail);
                        }
                    }

                }
            }

            return portfolioGroup;
        }

        public async Task<string> GetCheckNameMembercodeISExists(string _groupname, int _rf48_id, CancellationToken cancellationToken)
        {
            string result = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "p_group_name", OracleDbType = OracleDbType.Varchar2, Value = _groupname, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "p_rf48_id", OracleDbType = OracleDbType.Varchar2, Value = _rf48_id, Direction = ParameterDirection.Input });

            var portfolio = await _dbManager.GetStoredProcedureRefCursorAsync<PortfolioGroupViewModel>("pkg_portfolio_groups.ValidateGroupName", lstParams, "pview", cancellationToken);

            if (portfolio.Count > 0)
            {
                result = "Group name already exists";
            }

            return result;
        }

        public async Task<int> AddUpdatePortfolio(PortfolioGroupReqModel portfolioGroup, CancellationToken cancellationToken)
        {
            int portfolio_id = 0;
            List<OracleParameter> lstParams = new();

            if (portfolioGroup.um14_id > 0)
            {
                lstParams.Add(new OracleParameter { ParameterName = "p_Um14_id", OracleDbType = OracleDbType.Int32, Value = portfolioGroup.um14_id, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PUM14_NAME", OracleDbType = OracleDbType.Varchar2, Value = portfolioGroup.um14_group_name, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PUM14_NAME_SEC", OracleDbType = OracleDbType.Varchar2, Value = portfolioGroup.um14_group_description, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PUPDATED_BY", OracleDbType = OracleDbType.Int32, Value = portfolioGroup.um14_updated_by, Direction = ParameterDirection.Input });

                await _dbManager.ExecuteStoredProcedureAsync("pkg_portfolio_groups.Edit_data", lstParams, cancellationToken);
            }
            else
            {
                lstParams.Add(new OracleParameter { ParameterName = "PKey", OracleDbType = OracleDbType.Int32, Direction = ParameterDirection.Output });
                lstParams.Add(new OracleParameter { ParameterName = "PUM14_NAME", OracleDbType = OracleDbType.Varchar2, Value = portfolioGroup.um14_group_name, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PUM14_NAME_SEC", OracleDbType = OracleDbType.Varchar2, Value = portfolioGroup.um14_group_description, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PRF48_ID", OracleDbType = OracleDbType.Int32, Value = portfolioGroup.um14_rf48_id, Direction = ParameterDirection.Input });
                lstParams.Add(new OracleParameter { ParameterName = "PCREATED_BY", OracleDbType = OracleDbType.Int32, Value = portfolioGroup.um14_created_by, Direction = ParameterDirection.Input });

                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("pkg_portfolio_groups.add_data", lstParams, cancellationToken);
                portfolioGroup.um14_id = Convert.ToInt32(result.GetString("PKey"));
            }

            if (portfolioGroup.um14_id > 0)
            {
                var _batchid = portfolioGroup.uploadedList.Select(x => x.UM15_BATCH_ID).First();
                var _rf48_id = portfolioGroup.uploadedList.Select(x => x.UM14_RF48_ID).First();

                await AddDetails(_batchid, portfolioGroup.um14_id, cancellationToken);
            }

            return portfolio_id;
        }


        public async Task<bool> BulkInsertPortfolioAccount(List<PortfolioGroupValidate> portfolios, CancellationToken cancellationToken)
        {
            string query = "insert into UM15_PORTFOLIO_GROUPS_VALIDATE(um15_id,um15_security_account,um15_created_date,um15_batch_id,um15_status,um15_remarks,um15_um14_id,um15_created_by,um15_rf48_id)  " +
               "values (um15_portfolio_groups_seq.nextval,:p_um15_security_account,sysdate,:p_um15_batch_id,:p_um15_status,:p_um15_remarks,:p_um15_um14_id,:p_um15_created_by,:p_um15_rf48_id)";

            var parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter("p_um15_security_account", portfolios.Select(x => x.UM15_SECURITY_ACCOUNT).ToArray()));
            parameters.Add(new OracleParameter("p_um15_batch_id", portfolios.Select(x => x.UM15_BATCH_ID).ToArray()));
            parameters.Add(new OracleParameter("p_um15_status", portfolios.Select(x => x.UM15_STATUS).ToArray()));
            parameters.Add(new OracleParameter("p_um15_remarks", portfolios.Select(x => x.UM15_REMARKS).ToArray()));
            parameters.Add(new OracleParameter("p_um15_um14_id", portfolios.Select(x => x.UM15_UM14_ID).ToArray()));
            parameters.Add(new OracleParameter("p_um15_created_by", portfolios.Select(x => x.UM15_CREATED_BY).ToArray()));
            parameters.Add(new OracleParameter("p_um15_rf48_id", portfolios.Select(x => x.UM15_RF48_ID).ToArray()));

            return await _dbManager.BulkInsertAsync(query, parameters, portfolios.Count) == portfolios.Count;
        }

        public async Task AddDetails(int batchid, int um14_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new();

            lstParams.Add(new OracleParameter { ParameterName = "p_batch_id", OracleDbType = OracleDbType.Varchar2, Value = batchid, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "p_um14_id", OracleDbType = OracleDbType.Varchar2, Value = um14_id, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("pkg_portfolio_groups.add_details", lstParams, cancellationToken);
        }

        public List<PortfolioGroupValidate> GetPortfolioGroup(DataSet result, int batch_id, int rf48_id, int created_by, CancellationToken cancellationToken)
        {
            List<PortfolioGroupValidate> portfolioList = new List<PortfolioGroupValidate>();


            foreach (DataRow row in result?.Tables[0]?.Rows)
            {
                PortfolioGroupValidate portfolioGroup = new PortfolioGroupValidate();
                if (!string.IsNullOrEmpty(Convert.ToString(row["Account"])))
                {
                    portfolioGroup.UM15_SECURITY_ACCOUNT = row.Field<dynamic>("Account").ToString();
                    portfolioGroup.UM15_BATCH_ID = batch_id;
                    portfolioGroup.UM15_RF48_ID = rf48_id;
                    portfolioGroup.UM15_CREATED_BY = created_by;
                }
                portfolioList.Add(portfolioGroup);
            }

            return portfolioList;
        }

        public async Task<List<PortfolioGroupValidate>> ValidatePortfolioAccounts(int batch_id, int rf48_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new();

            lstParams.Add(new OracleParameter("pview", OracleDbType.RefCursor) { Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter("p_batch_id", OracleDbType.Int32) { Value = batch_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter("p_rf48_id", OracleDbType.Int32) { Value = rf48_id, Direction = ParameterDirection.Input });

            var portfolio = await _dbManager.GetStoredProcedureRefCursorAsync<PortfolioGroupValidate>("pkg_portfolio_groups.validate_portfolio_new", lstParams, "pview", cancellationToken);

            return portfolio;
        }

        public async Task<List<PortfolioGroupDeleteRes>> DeleteGroup(int um14_id, CancellationToken cancellationToken)
        {
            List<PortfolioGroupDeleteRes> result = new List<PortfolioGroupDeleteRes>();

            var parameters = new List<OracleParameter>();

            parameters.Add(new() { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            parameters.Add(new() { ParameterName = "gourpId", OracleDbType = OracleDbType.Varchar2, Value = um14_id, Direction = ParameterDirection.Input });

            result = await _dbManager.GetStoredProcedureRefCursorAsync<PortfolioGroupDeleteRes>("pkg_portfolio_groups.DeletePortfolioGroup", parameters, "pview", cancellationToken);

           
            return result;
        }
    }

}

