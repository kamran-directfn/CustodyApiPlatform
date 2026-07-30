using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.BankBranch;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Broker
{
    public sealed class BrokerRepository : IBrokerRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public BrokerRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<List<BrokerViewModel>> GetAllBrokersAsync(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<BrokerViewModel>("Pkg_RF07_BROKER.GET_DATA", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<BrokerViewModel> GetBrokerById(int brokerId, CancellationToken cancellationToken)
        {
            List<BrokerViewModel> data = await GetAllBrokersAsync(cancellationToken);

            BrokerViewModel broker = data.FirstOrDefault(x => x.RF07_BROKER_ID == brokerId);

            if (broker != null)
            {
                List<OracleParameter> lstParams = new List<OracleParameter>();

                lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

                List<BrokerContacts> broker_contacts = await _dbManager.GetStoredProcedureRefCursorAsync<BrokerContacts>("Pkg_RF47_BROKER_Accounts.GET_DATA", lstParams, "Pview", cancellationToken);

                broker.lstBrokerContacts = broker_contacts.Where(x => x.RF47_RF07_ID == broker.RF07_BROKER_ID).ToList();
            }

            return broker;
        }

        public async Task<List<BrokerViewModel>> UpdatePostStatus(int rf07_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PRF07_BROKER_ID", OracleDbType = OracleDbType.Int32, Value = rf07_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF07_POST_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF07_BROKER.UPDATE_POST_STATUS", lstParams, cancellationToken);

            List<BrokerViewModel> data = await GetAllBrokersAsync(cancellationToken);
            return data;
        }

        public async Task<List<BrokerViewModel>> DeleteBroker(int rf07_id, int user_id, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PRF07_BROKER_ID", OracleDbType = OracleDbType.Int32, Value = rf07_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PRF07_EDITED_BY", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });
            await _dbManager.ExecuteStoredProcedureAsync("Pkg_RF07_BROKER.DELETE_DATA", lstParams, cancellationToken);

            List<BrokerViewModel> data = await GetAllBrokersAsync(cancellationToken);
            return data;
        }

        public async Task<List<BrokerCache>> CacheBrokerData(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<BrokerCache>("Pkg_RF07_BROKER.GET_BROKER_CACHE", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<BrokerReqModel> SaveBroker(BrokerReqModel _broker, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REFERENCE_NO", Value = _broker.RF07_REFERENCE_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_RF01_ID", Value = _broker.RF07_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REPAID", Value = _broker.RF07_REPAID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_NAME", Value = _broker.RF07_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_NAME_SEC", Value = _broker.RF07_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_BIC_CODE", Value = _broker.RF07_BIC_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_PROP_ID", Value = _broker.RF07_PROP_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_CLIENT_POOL_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_PROP_SC_AC", Value = _broker.RF07_PROP_SC_AC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_CLIENT_POOL_SCE", Value = _broker.RF07_CLIENT_POOL_SCE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_TYPE", Value = _broker.RF07_TYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REMARKS", Value = _broker.RF07_REMARKS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_Edited_by", Value = _broker.RF07_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_Created_by", Value = _broker.RF07_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF07_BROKER.ADD_DATA", lstParams);

            _broker.RF07_BROKER_ID = Convert.ToInt32(result.GetString("PKey"));

            if(_broker.RF07_BROKER_ID > 0 && _broker.lstBrokerContact.Count> 0)
            {
                List<OracleParameter> oracleParameters = new List<OracleParameter>();

                oracleParameters.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
                oracleParameters.Add(new OracleParameter() { ParameterName = "PRF07_ID", Value = _broker.RF07_BROKER_ID,  Direction = System.Data.ParameterDirection.Input, });

                await _dbManager.ExecuteStoredProcedureAsync("PKG_RF47_BROKER_ACCOUNTS.DELETE_ALL_BROKER_CONTACTS", oracleParameters, cancellationToken);

                foreach (var contact in _broker.lstBrokerContact)
                {
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    parameters.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_RF07_ID", Value = _broker.RF07_BROKER_ID, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF07_ACCOUNT", Value = contact.RF07_ACCOUNT, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF07_ACCOUNT_TITLE", Value = contact.RF07_ACCOUNT_TITLE, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_IS_FROM_FILE", Value = 0, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_Edited_by", Value = _broker.RF07_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });

                    StoredProcedureResult result2 = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF47_BROKER_ACCOUNTS.ADD_DATA", parameters);
                }
            }

            return _broker;
        }

        public async Task<BrokerReqModel> UpdateBroker(BrokerReqModel _broker, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_BROKER_ID", Value = _broker.RF07_BROKER_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REFERENCE_NO", Value = _broker.RF07_REFERENCE_NO, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_RF01_ID", Value = _broker.RF07_RF01_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REPAID", Value = _broker.RF07_REPAID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_NAME", Value = _broker.RF07_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_NAME_SEC", Value = _broker.RF07_NAME_SEC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_BIC_CODE", Value = _broker.RF07_BIC_CODE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_PROP_ID", Value = _broker.RF07_PROP_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_CLIENT_POOL_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_PROP_SC_AC", Value = _broker.RF07_PROP_SC_AC, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_CLIENT_POOL_SCE", Value = _broker.RF07_CLIENT_POOL_SCE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_TYPE", Value = _broker.RF07_TYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_REMARKS", Value = _broker.RF07_REMARKS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PRF07_Edited_by", Value = _broker.RF07_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PError", Size = 32767, Direction = ParameterDirection.Output });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF07_BROKER.EDIT_DATA", lstParams);

          //  _broker.RF07_BROKER_ID = Convert.ToInt32(result.GetString("PKey"));

            if (_broker.RF07_BROKER_ID > 0 && _broker.lstBrokerContact.Count > 0)
            {
                List<OracleParameter> oracleParameters = new List<OracleParameter>();

                oracleParameters.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
                oracleParameters.Add(new OracleParameter() { ParameterName = "PRF07_ID", Value = _broker.RF07_BROKER_ID, Direction = System.Data.ParameterDirection.Input, });

                await _dbManager.ExecuteStoredProcedureAsync("PKG_RF47_BROKER_ACCOUNTS.DELETE_ALL_BROKER_CONTACTS", oracleParameters, cancellationToken);

                foreach (var contact in _broker.lstBrokerContact)
                {
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    parameters.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_RF07_ID", Value = _broker.RF07_BROKER_ID, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF07_ACCOUNT", Value = contact.RF07_ACCOUNT, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF07_ACCOUNT_TITLE", Value = contact.RF07_ACCOUNT_TITLE, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_IS_FROM_FILE", Value = 0, Direction = System.Data.ParameterDirection.Input, });
                    parameters.Add(new OracleParameter() { ParameterName = "PRF47_Edited_by", Value = _broker.RF07_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });

                    StoredProcedureResult result2 = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_RF47_BROKER_ACCOUNTS.ADD_DATA", parameters);
                }
            }

            return _broker;
        }
    }
}
