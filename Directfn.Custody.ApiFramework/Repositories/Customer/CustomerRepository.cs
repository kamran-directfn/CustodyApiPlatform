using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Customer;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Directfn.Custody.ApiFramework.Repositories.Common;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Repositories.Customer
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;
        public CustomerRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<string> CustomerPostUnpostStatus(int crm01_id, int isPosted, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PCRM01_ID", OracleDbType = OracleDbType.Int32, Value = crm01_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM01_STATUS", OracleDbType = OracleDbType.Int32, Value = isPosted, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_CRM01_CUSTOMER.UPDATE_CUSTOMERPOSTUNPOST", lstParams, cancellationToken);

            message = isPosted == 1 ? "Your Record has been Approved Successfully" : "Your Record has been marked Pending Successfully";
            return message;
        }

        public async Task<string> DeleteCustomer(int crm01_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM01_ID", OracleDbType = OracleDbType.Int32, Value = crm01_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM01_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_CRM01_CUSTOMER.DELETE_DATA", lstParams, cancellationToken);

            return message;
        }

        public async Task<List<CustomerBasicInfoViewModel>> GetAllCustomers(PaginationRequest<CustomerBasicInfoFilter> req, CancellationToken cancellationToken)
        {
            string filtersObj = JsonSerializer.Serialize(req.filter);

            var parameters = new CustomerBasicInfoFilter();
            parameters.PageNo = req.PageNo;
            parameters.PageSize = req.PageSize;
            parameters.memberCodeId = req.Filters.memberCodeId;
            parameters.PortfolioGroupId = req.Filters.PortfolioGroupId;

            List<CustomerBasicInfoViewModel> _lst = new List<CustomerBasicInfoViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            if (req.filter != null && !string.IsNullOrEmpty(req.filter.Trim()) && req.filter != "null")
            {
                FilterClass? filters = JsonSerializer.Deserialize<FilterClass>(req.filter);
                foreach (var item in filters.filters)
                {
                    switch (item.field.ToUpper())
                    {
                        case "CRM01_ACTIVE":
                            parameters.Crm01_Active = item.value;
                            break;

                        case "CRM01_EDAA_ID":
                            parameters.Crm01_Edaa_Id = item.value;
                            break;

                        case "CUSTOMER_TYPE":
                            parameters.Investor_Category = item.value;
                            break;

                        case "CRM01_LONG_NAME":
                            parameters.Long_Name = item.value;
                            break;

                        default:
                            break;
                    }
                }
            }

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_rf48_id", Value = parameters.memberCodeId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_number", Value = parameters.PageNo, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_size", Value = parameters.PageSize, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_activation_type", Value = parameters.Crm01_Active, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_edda_id", Value = parameters.Crm01_Edaa_Id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_investor_category", Value = parameters.Investor_Category, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_long_name", Value = parameters.Long_Name, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_group", Value = parameters.PortfolioGroupId, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<CustomerBasicInfoViewModel>("Pkg_CRM01_CUSTOMER.Get_Data_With_Filters", lstParams, "pview", cancellationToken);

            if (_lst.Any())
            {
                foreach (var account in _lst)
                {
                    switch (account.CRM01_ACTIVE)
                    {
                        case 1:
                            account.CRM01_ACTIVE_Desc = "ACTIVE";
                            break;
                        case 2:
                            account.CRM01_ACTIVE_Desc = "DEACTIVATED";
                            break;
                        case 3:
                            account.CRM01_ACTIVE_Desc = "SUSPENDED";
                            break;
                    }
                }
            }

            return _lst;
        }

        public async Task<CustomerBasicInfoViewModel> GetCustomerById(int Id, int memberCodeId, CancellationToken cancellationToken)
        {
            List<CustomerBasicInfoViewModel> _lst = new List<CustomerBasicInfoViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_rf48_id", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_customerId", Value = Id, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<CustomerBasicInfoViewModel>("Pkg_CRM01_CUSTOMER.Get_Data", lstParams, "pview", cancellationToken);

            return _lst.FirstOrDefault();
        }

        public async Task<CustomerBasicInfoReqModel> SaveCustomerBasicInfo(CustomerBasicInfoReqModel req, bool isSync, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();
            if (req.CRM01_ID > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_ID", Value = req.CRM01_ID, Direction = System.Data.ParameterDirection.Input, });
            }
            else
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            }

            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_CRM_NO", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_TYPE", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_ACTIVE", Value = req.CRM01_ACTIVE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_PARENT_ID", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_RF48_ID", Value = 2, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_LONG_NAME", Value = req.CRM01_LONG_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_FIRST_NAME", Value = req.CRM01_FIRST_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_ENG_LONG_NAME", Value = req.CRM01_ENG_LONG_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_ENG_SHORT_NAME", Value = req.CRM01_ENG_SHORT_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_TAX_RF09_ID", Value = req.CRM01_TAX_RF09_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_TAX_IDENTIFIER", Value = req.CRM01_TAX_IDENTIFIER, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_COMM_LANGUAGE", Value = req.CRM01_COMM_LANGUAGE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_COMM_METHOD", Value = req.CRM01_COMM_METHOD, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_INTERESTED_PARTY", Value = req.CRM01_INTERESTED_PARTY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_STAKEHOLDER_TYPE", Value = req.CRM01_STAKEHOLDER_TYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_DOB", Value = req.CRM01_DOB, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_RESIDENCE_RF09_ID", Value = req.CRM01_RESIDENCE_RF09_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_DECEASED_DATE", Value = req.CRM01_DECEASED_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_CITIZEN_RF09_ID", Value = req.CRM01_CITIZEN_RF09_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_RF32_ID", Value = req.CRM01_RF32_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_SECOND_NAME", Value = req.CRM01_SECOND_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_THIRD_NAME", Value = req.CRM01_THIRD_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_FOURTH_NAME", Value = req.CRM01_FOURTH_NAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_HJIRI_DOB", Value = req.CRM01_HJIRI_DOB, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_GENDER", Value = req.CRM01_GENDER, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_SH_INVESTOR_ID", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_IS_INDIVIDUAL", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_IS_SYNC", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_STATUS", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_RF66_ID", Value = 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_LEGAL_STATUS", Value = req.CRM01_LEGAL_STATUS, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_LEGAL_STATUS_DATE", Value = req.CRM01_LEGAL_STATUS_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_REG_RF09_ID", Value = req.CRM01_STAKEHOLDER_TYPE == "1" ? DBNull.Value : req.CRM01_REG_RF09_ID, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_INC_DATE", Value = req.CRM01_INC_DATE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDITED_BY", Value = req.CRM01_ID == 0 ? DBNull.Value : req.CRM01_MODIFIED_BY, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_CREATED_BY", Value = req.CRM01_CREATED_BY, Direction = System.Data.ParameterDirection.Input, });
            if (req.CRM01_IS_EDAA_CREATED > 0 && req.CRM01_ID > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_STATUS", Value = "Sync Required", Direction = System.Data.ParameterDirection.Input, });
            }
            else if (isSync == false && req.CRM01_ID == 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_STATUS", Value = "Pending", Direction = System.Data.ParameterDirection.Input, });
            }
            if (req.CRM01_IS_EDAA_CREATED == 0 && req.CRM01_ID > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_STATUS", Value = "Pending", Direction = System.Data.ParameterDirection.Input, });
            }
            if (isSync)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_STATUS", Value = "OK", Direction = System.Data.ParameterDirection.Input, });
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_IS_EDAA_CREATED", Value = "1", Direction = System.Data.ParameterDirection.Input, });
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_ID", Value = req.CRM01_EDAA_ID, Direction = System.Data.ParameterDirection.Input, });
            }
            else if (req.CRM01_ID == 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_IS_EDAA_CREATED", Value = 0, Direction = System.Data.ParameterDirection.Input, });
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EDAA_ID", Value = "", Direction = System.Data.ParameterDirection.Input, });
            }
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_IS_TAX_EXEMPT", Value = req.CRM01_IS_TAX_EXEMPT == true ? 1 : 0, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM01_EXEMPTION_VALID_UNTIL", Value = req.CRM01_EXEMPTION_VALID_UNTIL, Direction = System.Data.ParameterDirection.Input, });

            if (req.CRM01_ID > 0)
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM01_CUSTOMER.Edit_Data", lstParams);
            }
            else
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM01_CUSTOMER.Add_Data", lstParams);

                req.CRM01_ID = Convert.ToInt32(result.GetString("PKey"));
            }

            return req;
        }

        #region stackeHolder
        public async Task<StackeHolderViewModel> GetStakeById(int Id, CancellationToken cancellationToken)
        {
            List<StackeHolderViewModel> data = await GetStakeholderList(0, cancellationToken);

            StackeHolderViewModel _lst = data.FirstOrDefault(x => x.crm02_id == Id);

            return _lst;
        }

        public async Task<List<StackeHolderViewModel>> GetStakeholderList(int Id, CancellationToken cancellationToken)
        {
            List<StackeHolderViewModel> _lst = new List<StackeHolderViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<StackeHolderViewModel>("Pkg_CRM02_CUSTOMER_ID.Get_Data", lstParams, "pview", cancellationToken);

            if (Id > 0)
            {
                _lst = _lst.Where(x => x.crm02_status != -1 && x.crm02_crm01_id == Id).ToList();
            }

            return _lst;
        }
        public async Task<string> DeleteStakeholder(int crm02_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM02_ID", OracleDbType = OracleDbType.Int32, Value = crm02_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM02_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_CRM02_CUSTOMER_ID.DELETE_DATA", lstParams, cancellationToken);

            return message;
        }

        public async Task<StackeHolderReqModel> SaveStakeholder(StackeHolderReqModel req, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "PCRM02_ISSUING_ENTITY", Value = req.CRM02_ISSUING_ENTITY, Direction = ParameterDirection.Input });
            if (req.crm02_id > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_ID", Value = req.crm02_id, Direction = System.Data.ParameterDirection.Input, });
            }
            else
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            }
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_CRM01_ID", Value = req.crm02_crm01_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_RF23_TYPE_ID", Value = req.crm02_rf23_type_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_NUMBER", Value = req.crm02_number, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_ISSUE_DATE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_EXPIRE_DATE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_STATUS", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM02_Edited_by", Value = req.crm02_created_by, Direction = System.Data.ParameterDirection.Input, });

            if (req.crm02_id > 0)
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM02_CUSTOMER_ID.Edit_Data", lstParams);
            }
            else
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM02_CUSTOMER_ID.Add_Data", lstParams);

                req.crm02_id = Convert.ToInt32(result.GetString("PKey"));
            }

            return req;
        }

        #endregion stackeHolder

        #region AddressDetails
        public async Task<List<AddressDetailsViewModel>> GetAddressDetailsList(int Id, CancellationToken cancellationToken)
        {
            List<AddressDetailsViewModel> _lst = new List<AddressDetailsViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_CRM01_ID", Value = Id, Direction = System.Data.ParameterDirection.Input, });
            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<AddressDetailsViewModel>("Pkg_CRM04_ADDRESS.GET_DATA", lstParams, "pview", cancellationToken);

            return _lst;
        }

        public async Task<AddressDetailsViewModel> GetAddressById(int Id, CancellationToken cancellationToken)
        {
            List<AddressDetailsViewModel> _lst = new List<AddressDetailsViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "P_CRM04_ID", Value = Id, Direction = System.Data.ParameterDirection.Input, });
            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<AddressDetailsViewModel>("Pkg_CRM04_ADDRESS.GET_DATA_BY_ID", lstParams, "pview", cancellationToken);

            return _lst.FirstOrDefault();
        }

        public async Task<string> DeleteAddressDetails(int crm04_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM04_ID", OracleDbType = OracleDbType.Int32, Value = crm04_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM04_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_CRM04_ADDRESS.DELETE_DATA", lstParams, cancellationToken);

            return message;
        }

        public async Task<AddressDetailsReqModel> SaveAddressDetails(AddressDetailsReqModel req, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            if (req.crm04_id > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_ID", Value = req.crm04_id, Direction = System.Data.ParameterDirection.Input, });
            }
            else
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            }
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_CRM01_ID", Value = req.crm04_crm01_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_ADDRESS1", Value = req.crm04_address1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_ADDRESS2", Value = req.crm04_address2, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_ADDRESS3", Value = req.crm04_address3, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_POSTALCODE", Value = req.crm04_postalcode, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_RF09_ID", Value = req.crm04_rf09_id > 0 ? req.crm04_rf09_id : DBNull.Value, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_STATEPROVINCE", Value = req.crm04_stateprovince, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_RF10_ID", Value = req.crm04_rf10_id > 0 ? req.crm04_rf10_id : DBNull.Value, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_ADDRESSTYPE", Value = req.crm04_addresstype > 0 ? req.crm04_addresstype : DBNull.Value, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM04_EDITED_BY", Value = req.crm04_created_by, Direction = System.Data.ParameterDirection.Input, });

            if (req.crm04_id > 0)
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM04_ADDRESS.Edit_Data", lstParams);
            }
            else
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM04_ADDRESS.Add_Data", lstParams);

                req.crm04_id = Convert.ToInt32(result.GetString("PKey"));
            }

            return req;
        }

        #endregion AddressDetails

        #region ContactDetails

        public async Task<List<ContactDetailsViewModel>> GetContactDetailsList(int Id, CancellationToken cancellationToken)
        {
            List<ContactDetailsViewModel> _lst = new List<ContactDetailsViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<ContactDetailsViewModel>("Pkg_CRM03_CUSTOMER_CONTACT.Get_Data", lstParams, "pview", cancellationToken);

            if (Id > 0)
            {
                _lst = _lst.Where(x => x.crm03_status != -1 && x.crm03_crm01_id == Id).ToList();
            }

            return _lst;
        }

        public async Task<ContactDetailsViewModel> GetContactDetailsById(int Id, CancellationToken cancellationToken)
        {
            List<ContactDetailsViewModel> data = await GetContactDetailsList(0, cancellationToken);

            ContactDetailsViewModel _lst = data.FirstOrDefault(x => x.crm03_id == Id);

            return _lst;
        }

        public async Task<string> DeleteContactDetails(int crm03_id, int user_id, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<OracleParameter> lstParams = new List<OracleParameter>();
            lstParams.Add(new OracleParameter { ParameterName = "PKey", Size = 32767, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM03_ID", OracleDbType = OracleDbType.Int32, Value = crm03_id, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM03_Edited_by", OracleDbType = OracleDbType.Int32, Value = user_id, Direction = ParameterDirection.Input });

            await _dbManager.ExecuteStoredProcedureAsync("Pkg_CRM03_CUSTOMER_CONTACT.DELETE_DATA", lstParams, cancellationToken);

            return message;
        }

        public async Task<ContactDetailsReqModel> SaveContactDetails(ContactDetailsReqModel req, CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "PCRM03_TITLE", Value = req.crm03_title, Direction = ParameterDirection.Input });
            lstParams.Add(new OracleParameter { ParameterName = "PCRM03_PUBLIC_DETAIL", Value = req.crm03_public_detail, Direction = ParameterDirection.Input });
            if (req.crm03_id > 0)
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_ID", Value = req.crm03_id, Direction = System.Data.ParameterDirection.Input, });
            }
            else
            {
                lstParams.Add(new OracleParameter() { ParameterName = "PKey", Direction = System.Data.ParameterDirection.Output, Size = 32767 });
            }
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_CRM01_ID", Value = req.crm03_crm01_id, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_CONTACT_NAME", Value = req.crm03_contact_name, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_MAIN_CONTACT", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_EMAIL", Value = req.crm03_email, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_MOBILE_NO", Value = req.crm03_mobile_no, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_FAX_NO", Value = req.crm03_fax_no, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_WORK_NO", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_HOME_NO", Value = req.crm03_home_no, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_STATUS", Value = 1, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_CONTACT_NAME_SEC", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_IP", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_POSTAL_CODE", Value = null, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "PCRM03_Edited_by", Value = req.crm03_created_by, Direction = System.Data.ParameterDirection.Input, });

            if (req.crm03_id > 0)
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM03_CUSTOMER_CONTACT.Edit_Data", lstParams);
            }
            else
            {
                StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("Pkg_CRM03_CUSTOMER_CONTACT.Add_Data", lstParams);

                req.crm03_id = Convert.ToInt32(result.GetString("PKey"));
            }

            return req;
        }

        #endregion ContactDetails
    }
}