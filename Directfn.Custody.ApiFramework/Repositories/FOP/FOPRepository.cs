using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.GroupAccounts;
using Directfn.Custody.ApiFramework.Common.Enumerations;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Directfn.Custody.ApiFramework.Repositories.FOP
{
    public sealed class FOPRepository : IFOPRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        private readonly ICommonRepository _commonRepository;
        public FOPRepository(IOracleDbManagerAsync dbManager, ICommonRepository commonRepository)
        {
            _dbManager = dbManager;
            _commonRepository = commonRepository;
        }

        public async Task<List<FOPValidate>> FOPStagingTable(List<FOPValidate> _list, int batch_id, int rf48_id, int portfolioId, int created_by, CancellationToken cancellationToken)
        {
            foreach (var item in _list)
            {
                item.BatchId = batch_id;
                item.PRS50_CREATED_BY = created_by;
                item.PRS50_RF48_ID = rf48_id;
            }

            string query = "insert into PRS50_FOP_VALIDATION(prs50_id,prs50_trade_type,prs50_transfer_type,prs50_trade_date,prs50_sett_date,prs50_isin,prs50_quantity_type,prs50_quantity,prs50_unique_reference,prs50_sender_custodian,prs50_acc_sender_custodian,prs50_receiver_custodian,prs50_acc_receiver_custodian,prs50_created_by,prs50_created_date,prs50_rf48_id,prs50_rf42_id,is_valid,prs50_validation_status)  " +
               "values (FOP_VALID_SEQ.nextval,:p_prs50_trade_type,:p_prs50_transfer_type,:p_prs50_trade_date,:p_prs50_sett_date,:p_prs50_isin,:p_prs50_quantity_type,:p_prs50_quantity,:p_prs50_unique_reference,:p_prs50_sender_custodian, :p_prs50_acc_sender_custodian, :p_prs50_receiver_custodian, :p_prs50_acc_receiver_custodian, :p_prs50_created_by, sysdate, :p_prs50_rf48_id, :p_prs50_rf42_id, :p_is_valid, :p_prs50_validation_status)";

            var parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter("p_prs50_trade_type", _list.Select(x => x.PRS50_TRADE_TYPE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_transfer_type", _list.Select(x => x.PRS50_TRANSFER_TYPE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_trade_date", _list.Select(x => x.PRS50_TRADE_DATE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_sett_date", _list.Select(x => x.PRS50_SETT_DATE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_isin", _list.Select(x => x.PRS50_ISIN).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_quantity_type", _list.Select(x => x.PRS50_QUANTITY_TYPE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_quantity", _list.Select(x => x.PRS50_QUANTITY).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_unique_reference", _list.Select(x => x.PRS50_UNIQUE_REFERENCE).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_sender_custodian", _list.Select(x => x.PRS50_SENDER_CUSTODIAN).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_acc_sender_custodian", _list.Select(x => x.PRS50_ACC_SENDER_CUSTODIAN).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_receiver_custodian", _list.Select(x => x.PRS50_RECEIVER_CUSTODIAN).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_acc_receiver_custodian", _list.Select(x => x.PRS50_ACC_RECEIVER_CUSTODIAN).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_created_by", _list.Select(x => x.PRS50_CREATED_BY).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_rf48_id", _list.Select(x => x.PRS50_RF48_ID).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_rf42_id", _list.Select(x => x.BatchId).ToArray()));
            parameters.Add(new OracleParameter("p_is_valid", _list.Select(x => x.IS_VALID).ToArray()));
            parameters.Add(new OracleParameter("p_prs50_validation_status", _list.Select(x => x.PRS50_Validation_Status).ToArray()));

            bool inserted = await _dbManager.BulkInsertAsync(query, parameters, _list.Count, cancellationToken) == _list.Count;

            if (!inserted)
            {
                _list.Clear();
            }

            return _list;
        }

        public async Task<List<FOPValidate>> GetFOP(DataSet result, int rf48_id, int portfolioId, int created_by, CancellationToken cancellationToken)
        {
            List<FOPValidate> listFOP = new List<FOPValidate>();

            DataTable FOP_DT = result.Tables[0];

            if (FOP_DT != null && FOP_DT.Rows.Count > 0)  //&& result.Rows.Count > 0
            {
                for (int i = 0; i < FOP_DT.Rows.Count; i++)
                {
                    FOPValidate _fop = new FOPValidate();
                    _fop.IS_VALID = 1;
                    _fop.PRS50_Validation_Status = "Valid.";
                    _fop.isShownAtGrid = 1;
                    _fop.PRS50_TRADE_TYPE = Convert.ToString(result.Tables[0].Rows[i]["TRADE_TYPE"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_TRADE_TYPE))
                    {
                        _fop.PRS50_Validation_Status = "Trade Type is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_TRADE_TYPE.ToUpper() != "DF" && _fop.PRS50_TRADE_TYPE.ToUpper() != "RF" && _fop.PRS50_TRADE_TYPE.ToUpper() != "BOTH")
                        {
                            _fop.PRS50_Validation_Status = "Trade Type should be one of DF/RF/BOTH.";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_TRADE_DATE = Convert.ToString(result.Tables[0].Rows[i]["TRADE_DATE"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_TRADE_DATE))
                    {
                        _fop.PRS50_Validation_Status += "Trade Date is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_TRADE_DATE.Length != 8)
                        {
                            _fop.PRS50_Validation_Status += "Trade Date format should be YYYYMMDD";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_SETT_DATE = Convert.ToString(result.Tables[0].Rows[i]["SETT_DATE"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_SETT_DATE))
                    {
                        _fop.PRS50_Validation_Status += "Settlement Date is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_SETT_DATE.Length != 8)
                        {
                            _fop.PRS50_Validation_Status += "Settlement Date format should be YYYYMMDD";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_ISIN = Convert.ToString(result.Tables[0].Rows[i]["ISIN"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_ISIN))
                    {
                        _fop.PRS50_Validation_Status += "ISIN is required.";
                        _fop.IS_VALID = -1;
                    }

                    _fop.PRS50_QUANTITY_TYPE = Convert.ToString(result.Tables[0].Rows[i]["QUANTITY_TYPE"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_QUANTITY_TYPE))
                    {
                        _fop.PRS50_Validation_Status = "Quantity type is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_QUANTITY_TYPE.ToUpper() != "UNIT" && _fop.PRS50_QUANTITY_TYPE.ToUpper() != "FAMT")
                        {
                            _fop.PRS50_Validation_Status = "Quantity type should be one of UNIT/FAMT";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_QUANTITY = Convert.ToInt64(result.Tables[0].Rows[i]["QUANTITY"]);
                    if (_fop.PRS50_QUANTITY < 0)
                    {
                        _fop.PRS50_Validation_Status += "Qty is required.";
                        _fop.IS_VALID = -1;
                    }

                    _fop.PRS50_UNIQUE_REFERENCE = Convert.ToString(result.Tables[0].Rows[i]["UNIQUE_REFERENCE"]);
                    if (listFOP.Any(x => x.PRS50_UNIQUE_REFERENCE == _fop.PRS50_UNIQUE_REFERENCE))
                    {
                        _fop.PRS50_Validation_Status += "Unique Reference Should be Unique.";
                        _fop.IS_VALID = -1;
                    }

                    if (string.IsNullOrEmpty(_fop.PRS50_UNIQUE_REFERENCE))
                    {
                        _fop.PRS50_Validation_Status += "Unique Reference is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_UNIQUE_REFERENCE.Length > 16)
                        {
                            _fop.PRS50_Validation_Status += "Unique Reference length should be of max 16 characters.";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_SENDER_CUSTODIAN = Convert.ToString(result.Tables[0].Rows[i]["SENDER_CUSTODIAN"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_SENDER_CUSTODIAN))
                    {
                        _fop.PRS50_Validation_Status += " Sender Custodian code is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_SENDER_CUSTODIAN.Length != 3)
                        {
                            _fop.PRS50_Validation_Status += "Sender Custodian code length shall be of 3 characters.";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_ACC_SENDER_CUSTODIAN = Convert.ToString(result.Tables[0].Rows[i]["ACCOUNT_WITH_SENDER_CUSTODIAN"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_ACC_SENDER_CUSTODIAN))
                    {
                        _fop.PRS50_Validation_Status += "Sender Custodian account is required.";
                        _fop.IS_VALID = -1;
                    }

                    _fop.PRS50_RECEIVER_CUSTODIAN = Convert.ToString(result.Tables[0].Rows[i]["RECEIVER_CUSTODIAN"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_RECEIVER_CUSTODIAN))
                    {
                        _fop.PRS50_Validation_Status += "Receiver Custodian account is required.";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        if (_fop.PRS50_RECEIVER_CUSTODIAN.Length != 3)
                        {
                            _fop.PRS50_Validation_Status += "Receiver Custodian code length shall be of 3 characters.";
                            _fop.IS_VALID = -1;
                        }
                    }

                    _fop.PRS50_ACC_RECEIVER_CUSTODIAN = Convert.ToString(result.Tables[0].Rows[i]["ACCOUNT_WITH_RECEIVER_CUSTODIAN"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_ACC_RECEIVER_CUSTODIAN))
                    {
                        _fop.PRS50_Validation_Status += "Receiver Custodian account is required.";
                        _fop.IS_VALID = -1;

                    }

                    _fop.PRS50_TRANSFER_TYPE = Convert.ToString(result.Tables[0].Rows[i]["TRANSFER_TYPE"]);
                    if (string.IsNullOrEmpty(_fop.PRS50_TRANSFER_TYPE))
                    {
                        _fop.PRS50_Validation_Status += "Transfer Type is required";
                        _fop.IS_VALID = -1;
                    }
                    else
                    {
                        var temp = Enumerations.GetFopTransferTypes().FirstOrDefault(x => x.Key == _fop.PRS50_TRANSFER_TYPE.ToUpper().Trim());
                        if (temp.Key == null)
                        {
                            _fop.IS_VALID = -1;
                            _fop.PRS50_Validation_Status += "Transfer Type is not valid.";
                        }
                    }

                    if (_fop.IS_VALID == -1)
                    {
                        _fop.PRS50_Validation_Status = _fop.PRS50_Validation_Status.Replace("Valid.", "");
                    }
                    _fop.GuId = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 11);
                    listFOP.Add(_fop);
                }
            }

            var groupsValidation = listFOP.Where(x => x.IS_VALID >= 1).ToList();
            if (groupsValidation.Count > 0)
            {
                List<GroupAccounts> listAllowedAccounts = new List<GroupAccounts>();
                listAllowedAccounts = await _commonRepository.GetPortfolioAccountsByUser(portfolioId, rf48_id, cancellationToken);
                var _allowedAccounts = listAllowedAccounts.Select(x => x.account).ToList();

                foreach (var group in groupsValidation)
                {
                    if (group.PRS50_TRADE_TYPE.ToUpper() == "BOTH")
                    {
                        if (!_allowedAccounts.Contains(group.PRS50_ACC_SENDER_CUSTODIAN))
                        {
                            group.IS_VALID = -1;
                            group.PRS50_Validation_Status += " Sender Account does not belong to your portfolio group.";
                        }
                        if (!_allowedAccounts.Contains(group.PRS50_ACC_RECEIVER_CUSTODIAN))
                        {
                            group.IS_VALID = -1;
                            group.PRS50_Validation_Status += " Receiver Account does not belong to your portfolio group.";
                        }
                    }
                    if (group.PRS50_TRADE_TYPE.ToUpper() == "DF")
                    {
                        if (!_allowedAccounts.Contains(group.PRS50_ACC_SENDER_CUSTODIAN))
                        {
                            group.IS_VALID = -1;
                            group.PRS50_Validation_Status += " Sender Account does not belong to your portfolio group.";
                        }
                    }
                    if (group.PRS50_TRADE_TYPE.ToUpper() == "RF")
                    {
                        if (!_allowedAccounts.Contains(group.PRS50_ACC_RECEIVER_CUSTODIAN))
                        {
                            group.IS_VALID = -1;
                            group.PRS50_Validation_Status += " Receiver Account does not belong to your portfolio group.";
                        }
                    }
                    if (group.IS_VALID < 1)
                    {
                        group.PRS50_Validation_Status = group.PRS50_Validation_Status.Replace("Valid.", "");
                    }
                }
            }

            return groupsValidation;
        }

        public async Task<List<FOPValidate>> DistinctUniqueRef(List<FOPValidate> _list, CancellationToken cancellationToken)
        {
            List<FOPValidate> fops = new List<FOPValidate>();
            List<OracleParameter> parameters = new List<OracleParameter>();

            parameters.Add(new OracleParameter { ParameterName = "Pview", Direction = ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
            fops = await _dbManager.GetStoredProcedureRefCursorAsync<FOPValidate>("PKG_PRS50_FOP.DISTINCT_UNIQUEREF", parameters, "Pview", cancellationToken);

            // bool isUniqueExsit = _list.OrderBy(x => x.PRS50_UNIQUE_REFERENCE).SequenceEqual(fops.OrderBy(y => y.PRS50_UNIQUE_REFERENCE));

            foreach (var item in _list)
            {
                if (fops.FirstOrDefault(y => item.PRS50_UNIQUE_REFERENCE == y.PRS50_UNIQUE_REFERENCE) != null)
                {
                    item.IS_VALID = -1;
                    item.PRS50_Validation_Status = item.PRS50_Validation_Status + " || Unique Reference already exists. Kindly change the Unique Reference.";
                }
            }

            return _list;
        }

        public async Task<List<FOPValidate>> GetValidFOPByBatchId(int batchId, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "P_batchId", Value = batchId, Direction = ParameterDirection.Input });

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<FOPValidate>("PKG_PRS50_FOP.Get_FOP_By_Batch_Id", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<string> SaveFOP(List<FOPValidate> _list, string MemberCode, CancellationToken cancellationToken)
        {
            string message = string.Empty;

            if (_list.Any())
            {


                bool result = _list.Any(x => x.IS_VALID == -1);

                if (result)
                {
                    message = "One or more records are invalid. Please review the records and try again.";
                }
                else
                {
                    List<FOPValidate> _lstBoth = new List<FOPValidate>();
                    foreach (var item in _list)
                    {
                        item.PRS50_Message_Id = _commonRepository.GetReqId();
                        //item.PRS50_CREATED_BY = User.Id;
                        //item.PRS50_RF48_ID = User.MemberCodeID;
                        item.PRS50_PLACE_OF_STLMNT = _commonRepository.EDAA_BIC();

                        if (item.PRS50_TRADE_TYPE.ToUpper() == "BOTH")
                        {
                            ///////// First generate the RF
                            FOPValidate _fop_RF = new FOPValidate();
                            _fop_RF.IS_VALID = 1;
                            _fop_RF.PRS50_TRADE_TYPE = "RF";
                            _fop_RF.PRS50_Validation_Status = "Valid";
                            _fop_RF.PRS50_UNIQUE_REFERENCE = item.PRS50_UNIQUE_REFERENCE;
                            _fop_RF.PRS50_Message_Id = _commonRepository.GetReqId();
                            _fop_RF.PRS50_MODIFIED_BY = item.PRS50_CREATED_BY;
                            _fop_RF.PRS50_MODIFIED_DATE = DateTime.Now;
                            _fop_RF.PRS50_CREATED_BY = item.PRS50_CREATED_BY;
                            _fop_RF.PRS50_CREATED_DATE = DateTime.Now;
                            _fop_RF.PRS50_RECEIVER_CUSTODIAN = item.PRS50_RECEIVER_CUSTODIAN;
                            _fop_RF.PRS50_SENDER_CUSTODIAN = item.PRS50_SENDER_CUSTODIAN;
                            _fop_RF.PRS50_ACC_RECEIVER_CUSTODIAN = item.PRS50_ACC_RECEIVER_CUSTODIAN;
                            _fop_RF.PRS50_ACC_SENDER_CUSTODIAN = item.PRS50_ACC_SENDER_CUSTODIAN;
                            _fop_RF.PRS50_QUANTITY = item.PRS50_QUANTITY;
                            _fop_RF.PRS50_SETT_DATE = item.PRS50_SETT_DATE;
                            _fop_RF.PRS50_TRADE_DATE = item.PRS50_TRADE_DATE;
                            _fop_RF.PRS50_ISIN = item.PRS50_ISIN;
                            _fop_RF.PRS50_QUANTITY_TYPE = item.PRS50_QUANTITY_TYPE;
                            _fop_RF.PRS50_TRANSFER_TYPE = item.PRS50_TRANSFER_TYPE;
                            _fop_RF.PRS50_RF48_ID = item.PRS50_RF48_ID;
                            _lstBoth.Add(_fop_RF);

                            ///////// Secondly generate the DF
                            FOPValidate _fop_DF = new FOPValidate();
                            _fop_DF.IS_VALID = 1;
                            _fop_DF.PRS50_TRADE_TYPE = "DF";
                            _fop_DF.PRS50_Validation_Status = "Valid";
                            _fop_DF.PRS50_UNIQUE_REFERENCE = item.PRS50_UNIQUE_REFERENCE;
                            _fop_DF.PRS50_Message_Id = _commonRepository.GetReqId();
                            _fop_DF.PRS50_MODIFIED_BY = item.PRS50_CREATED_BY;
                            _fop_DF.PRS50_MODIFIED_DATE = DateTime.Now;
                            _fop_DF.PRS50_CREATED_BY = item.PRS50_CREATED_BY;
                            _fop_DF.PRS50_CREATED_DATE = DateTime.Now;
                            _fop_DF.PRS50_RECEIVER_CUSTODIAN = item.PRS50_RECEIVER_CUSTODIAN;
                            _fop_DF.PRS50_SENDER_CUSTODIAN = item.PRS50_SENDER_CUSTODIAN;
                            _fop_DF.PRS50_ACC_RECEIVER_CUSTODIAN = item.PRS50_ACC_RECEIVER_CUSTODIAN;
                            _fop_DF.PRS50_ACC_SENDER_CUSTODIAN = item.PRS50_ACC_SENDER_CUSTODIAN;
                            _fop_DF.PRS50_QUANTITY = item.PRS50_QUANTITY;
                            _fop_DF.PRS50_SETT_DATE = item.PRS50_SETT_DATE;
                            _fop_DF.PRS50_TRADE_DATE = item.PRS50_TRADE_DATE;
                            _fop_DF.PRS50_ISIN = item.PRS50_ISIN;
                            _fop_DF.PRS50_QUANTITY_TYPE = item.PRS50_QUANTITY_TYPE;
                            _fop_DF.PRS50_TRANSFER_TYPE = item.PRS50_TRANSFER_TYPE;
                            _fop_DF.PRS50_RF48_ID = item.PRS50_RF48_ID;
                            _lstBoth.Add(_fop_DF);
                        }
                    }

                    _list.AddRange(_lstBoth);

                    foreach (var item in _list)
                    {

                        List<OracleParameter> lstParams = new List<OracleParameter>();
                        item.PRS50_RF42_ID = item.PRS50_RF42_ID;
                        if (item.PRS50_ID > 0)
                        {
                            lstParams.Add(new OracleParameter { ParameterName = "PPRS50_ID", Value = item.PRS50_ID, Direction = ParameterDirection.Input });
                        }
                        else
                        {
                            lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
                        }

                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_TRADE_TYPE", Value = item.PRS50_TRADE_TYPE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_TRADE_DATE", Value = item.PRS50_TRADE_DATE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_SETT_DATE", Value = item.PRS50_SETT_DATE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_ISIN", Value = item.PRS50_ISIN, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_BROKER", Value = item.PRS50_BROKER, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_QUANTITY", Value = item.PRS50_QUANTITY, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_UNIQUE_REFERENCE", Value = item.PRS50_UNIQUE_REFERENCE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_SENDER_CUSTODIAN", Value = item.PRS50_SENDER_CUSTODIAN, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_ACC_SENDER_CUSTODIAN", Value = item.PRS50_ACC_SENDER_CUSTODIAN, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_RECEIVER_CUSTODIAN", Value = item.PRS50_RECEIVER_CUSTODIAN, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_ACC_RECEIVER_CUSTODIAN", Value = item.PRS50_ACC_RECEIVER_CUSTODIAN, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_IS_SENT", Value = item.PRS50_IS_SENT, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_IS_ACKNOWLEDGE", Value = item.PRS50_IS_ACKNOWLEDGE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_STATUS", Value = 2, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_CREATED_BY", Value = item.PRS50_CREATED_BY, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_MODIFIED_BY", Value = item.PRS50_MODIFIED_BY, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_RF48_ID", Value = item.PRS50_RF48_ID, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_RF42_ID", Value = item.PRS50_RF42_ID, Direction = ParameterDirection.Input });

                        string reqMsg;

                        if (item.PRS50_TRADE_TYPE.ToUpper() == "DF")
                        {
                            reqMsg = Generate_DF_Msg(item, MemberCode);
                        }
                        else if (item.PRS50_TRADE_TYPE.ToUpper() == "RF")
                        {
                            reqMsg = Generate_RF_Msg(item, MemberCode);
                        }
                        else
                        {
                            reqMsg = string.Empty;
                        }

                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_Req_Msg", Value = reqMsg, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_MESSAGE_ID", Value = item.PRS50_Message_Id, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_TRANSFER_TYPE", Value = item.PRS50_TRANSFER_TYPE, Direction = ParameterDirection.Input });
                        lstParams.Add(new OracleParameter { ParameterName = "PPRS50_QUANTITY_TYPE", Value = item.PRS50_QUANTITY_TYPE, Direction = ParameterDirection.Input });

                        if (item.PRS50_ID > 0 && item.PRS50_TRADE_TYPE != "BOTH")
                        {
                            StoredProcedureResult data = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_PRS50_FOP.Edit_Data", lstParams);
                        }
                        else
                        {
                            if (item.PRS50_TRADE_TYPE != "BOTH")
                            {
                                StoredProcedureResult result_data = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_PRS50_FOP.Add_Data", lstParams);
                            }
                        }
                    }

                    message = "Successfully Uploaded";
                }
            }
            else
            {
                message = "No record found";
            }

            return message;
        }

        public string Generate_RF_Msg(FOPValidate _rf, string memberCode)
        {
            string _tagSETR = "";
            _tagSETR = Enumerations.GetFopTransferTypes().FirstOrDefault(x => x.Key == _rf.PRS50_TRANSFER_TYPE.ToUpper().Trim()).Value;


            string MT540 = _commonRepository.GetMT5BasicHeaderBlock() + _commonRepository.Get_Header_Block("540", memberCode) + "{4:" + Environment.NewLine +
":16R:GENL" + Environment.NewLine +
":20C::SEME//" + _rf.PRS50_Message_Id + Environment.NewLine +
":23G:NEWM" + Environment.NewLine +
":16S:GENL" + Environment.NewLine +
":16R:TRADDET" + Environment.NewLine +
":98A::SETT//" + _rf.PRS50_SETT_DATE + Environment.NewLine +
":98A::TRAD//" + _rf.PRS50_TRADE_DATE + Environment.NewLine +
":35B:ISIN " + _rf.PRS50_ISIN + Environment.NewLine +
":16S:TRADDET" + Environment.NewLine +
":16R:FIAC" + Environment.NewLine +
":36B::SETT//" + _rf.PRS50_QUANTITY_TYPE + "/" + _rf.PRS50_QUANTITY + "," + Environment.NewLine +
":97A::SAFE//" + _rf.PRS50_ACC_RECEIVER_CUSTODIAN + Environment.NewLine +
":16S:FIAC" + Environment.NewLine +
":16R:SETDET" + Environment.NewLine +
":22F::SETR//" + _tagSETR + Environment.NewLine +
":22F::RTGS//YRTG" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95R::DEAG/CCSD/" + _rf.PRS50_SENDER_CUSTODIAN + Environment.NewLine +
":97A::SAFE//" + _rf.PRS50_ACC_SENDER_CUSTODIAN + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95R::REAG/CCSD/" + _rf.PRS50_RECEIVER_CUSTODIAN + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95P::PSET//" + _commonRepository.EDAA_BIC() + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16S:SETDET" + Environment.NewLine +
"-}";
            return MT540;

        }

        public string Generate_DF_Msg(FOPValidate _df, string memberCode)
        {
            string _tagSETR = "";
            _tagSETR = Enumerations.GetFopTransferTypes().FirstOrDefault(x => x.Key == _df.PRS50_TRANSFER_TYPE.ToUpper().Trim()).Value;

            string MT542 = _commonRepository.GetMT5BasicHeaderBlock() + _commonRepository.Get_Header_Block("542", memberCode) + "{4:" + Environment.NewLine +
":16R:GENL" + Environment.NewLine +
":20C::SEME//" + _df.PRS50_Message_Id + Environment.NewLine +
":23G:NEWM" + Environment.NewLine +
":16S:GENL" + Environment.NewLine +
":16R:TRADDET" + Environment.NewLine +
":98A::SETT//" + _df.PRS50_SETT_DATE + Environment.NewLine +
":98A::TRAD//" + _df.PRS50_TRADE_DATE + Environment.NewLine +
":35B:ISIN " + _df.PRS50_ISIN + Environment.NewLine +
":16S:TRADDET" + Environment.NewLine +
":16R:FIAC" + Environment.NewLine +
":36B::SETT//" + _df.PRS50_QUANTITY_TYPE + "/" + _df.PRS50_QUANTITY + "," + Environment.NewLine +
":97A::SAFE//" + _df.PRS50_ACC_SENDER_CUSTODIAN + Environment.NewLine +
":16S:FIAC" + Environment.NewLine +
":16R:SETDET" + Environment.NewLine +
":22F::SETR//" + _tagSETR + Environment.NewLine +
":22F::RTGS//YRTG" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95R::DEAG/CCSD/" + _df.PRS50_SENDER_CUSTODIAN + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95R::REAG/CCSD/" + _df.PRS50_RECEIVER_CUSTODIAN + Environment.NewLine +
":97A::SAFE//" + _df.PRS50_ACC_RECEIVER_CUSTODIAN + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16R:SETPRTY" + Environment.NewLine +
":95P::PSET//" + _commonRepository.EDAA_BIC() + Environment.NewLine +
":16S:SETPRTY" + Environment.NewLine +
":16S:SETDET" + Environment.NewLine +
"-}";
            return MT542;

        }
    }
}
