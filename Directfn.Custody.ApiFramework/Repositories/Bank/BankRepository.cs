using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Bank
{
    public sealed class BankRepository : IBankRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public BankRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<BanksViewModel>> GetAllBanksAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            List<BanksViewModel> banks = await _dbManager.GetStoredProcedureRefCursorAsync<BanksViewModel>("PKG_RF03_BANKS.GET_DATA", lstParams, "pview", cancellationToken);

            return banks;
        }

        public async Task<BanksViewModel> GetBankById(int bankId, CancellationToken cancellationToken)
        {
            List<BanksViewModel> data = await GetAllBanksAsync(cancellationToken);

            BanksViewModel bank = data.FirstOrDefault(x => x.RF03_ID == bankId);

            return bank;
        }

        public async Task<List<BanksViewModel>> UpdatePostStatus(int rf03_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF03_ID", OracleDbType = OracleDbType.Int32, Value = rf03_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF03_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF03_BANKS.Update_Post_Status", lstParams, cancellationToken);

            List<BanksViewModel> data = await GetAllBanksAsync(cancellationToken);
            return data;
        }

        public async Task<List<BanksViewModel>> DeleteBank(int rf03_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF03_ID", OracleDbType = OracleDbType.Int32, Value = rf03_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF03_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF03_BANKS.Delete_Data", lstParams, cancellationToken);

            List<BanksViewModel> data = await GetAllBanksAsync(cancellationToken);
            return data;
        }

        public async Task<BankReqModel> SaveBank(BankReqModel bank, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_NAME", Value = bank.RF03_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_NAME_SEC", Value = bank.RF03_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_CONTACT_NOS", Value = bank.RF03_CONTACT_NOS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_ADDRESS", Value = bank.RF03_ADDRESS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_SWIFT_CODE", Value = bank.RF03_SWIFT_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_INSTITUTION_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_IS_ISLAMIC", Value = bank.RF03_IS_ISLAMIC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_ENABLE_ONLINE_TRANSFERS", Value = bank.RF03_ENABLE_ONLINE_TRANSFERS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_EXTERNAL_REF", Value = bank.RF03_EXTERNAL_REF, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_CODE", Value = bank.RF03_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_Edited_by", Value = bank.RF03_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF03_BANKS.Add_Data", lstParams);

            bank.RF03_ID = Convert.ToInt32(result.GetString("PKey"));

            return bank;
        }

        public async Task<BankReqModel> UpdateBank(BankReqModel bank, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_NAME", Value = bank.RF03_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_NAME_SEC", Value = bank.RF03_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_CONTACT_NOS", Value = bank.RF03_CONTACT_NOS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_ADDRESS", Value = bank.RF03_ADDRESS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_SWIFT_CODE", Value = bank.RF03_SWIFT_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_INSTITUTION_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_IS_ISLAMIC", Value = bank.RF03_IS_ISLAMIC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_ENABLE_ONLINE_TRANSFERS", Value = bank.RF03_ENABLE_ONLINE_TRANSFERS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_EXTERNAL_REF", Value = bank.RF03_EXTERNAL_REF, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_CODE", Value = bank.RF03_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_ID", Value = bank.RF03_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF03_Edited_by", Value = bank.RF03_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PError", Direction = System.Data.ParameterDirection.Output, Size = 32767 });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_RF03_BANKS.Edit_Data", lstParams);

            return bank;
        }
    }
}
