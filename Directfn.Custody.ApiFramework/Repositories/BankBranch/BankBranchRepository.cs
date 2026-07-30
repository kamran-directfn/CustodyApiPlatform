using Directfn.Custody.ApiFramework.Common.DTOs.BankBranch;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.BankBranch
{
    public sealed class BankBranchRepository : IBankBranchRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public BankBranchRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<BankBranchViewModel>> GetAllBranchesAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<BankBranchViewModel> _branches = await _dbManager.GetStoredProcedureRefCursorAsync<BankBranchViewModel>("Pkg_RF04_BANK_BRANCHES.GET_DATA", lstParams, "pview", cancellationToken);

            return _branches;
        }

        public async Task<BankBranchViewModel> GetBranchById(int branchkId, CancellationToken cancellationToken)
        {
            List<BankBranchViewModel> data = await GetAllBranchesAsync(cancellationToken);

            BankBranchViewModel branch = data.FirstOrDefault(x => x.RF04_ID == branchkId);

            return branch;
        }

        public async Task<List<BankBranchViewModel>> UpdatePostStatus(int rf04_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "Prf04_ID", OracleDbType = OracleDbType.Int32, Value = rf04_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "Prf04_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF04_BANK_BRANCHES.Update_Post_Status", lstParams, cancellationToken);

            List<BankBranchViewModel> data = await GetAllBranchesAsync(cancellationToken);
            return data;
        }

        public async Task<List<BankBranchViewModel>> DeleteBank(int rf04_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF04_ID", OracleDbType = OracleDbType.Int32, Value = rf04_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF04_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF04_BANK_BRANCHES.Delete_Data", lstParams, cancellationToken);

            List<BankBranchViewModel> data = await GetAllBranchesAsync(cancellationToken);
            return data;
        }

        public async Task<BankBranchReqModel> SaveBank(BankBranchReqModel branch, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_NAME_SEC", Value = branch.RF04_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_CODE", Value = branch.RF04_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_NAME", Value = branch.RF04_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_ADDRESS", Value = branch.RF04_ADDRESS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_TEL", Value = branch.RF04_TEL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_EXTERNAL_REF", Value = branch.RF04_EXTERNAL_REF, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_RF03_ID", Value = branch.RF04_RF03_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_ACTIVE", Value = branch.RF04_ACTIVE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_Edited_by", Value = branch.RF04_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_Created_by", Value = branch.RF04_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
           
            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF04_BANK_BRANCHES.Add_Data", lstParams);

            branch.RF04_ID = Convert.ToInt32(result.GetString("PKey"));

            return branch;
        }

        public async Task<BankBranchReqModel> UpdateBank(BankBranchReqModel branch, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_NAME_SEC", Value = branch.RF04_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_CODE", Value = branch.RF04_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_NAME", Value = branch.RF04_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_ADDRESS", Value = branch.RF04_ADDRESS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_TEL", Value = branch.RF04_TEL, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_EXTERNAL_REF", Value = branch.RF04_EXTERNAL_REF, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_ID", Value = branch.RF04_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_RF03_ID", Value = branch.RF04_RF03_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_ACTIVE", Value = branch.RF04_ACTIVE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF04_Edited_by", Value = branch.RF04_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PError", Direction = System.Data.ParameterDirection.Output, Size = 32767 });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF04_BANK_BRANCHES.Edit_Data", lstParams);

            return branch;
        }
       
    }
}
