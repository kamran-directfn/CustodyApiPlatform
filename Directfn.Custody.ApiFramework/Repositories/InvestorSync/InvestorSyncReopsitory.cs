
using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Customer;
using Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.Database.Results;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.Customer;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.Json;

namespace Directfn.Custody.ApiFramework.Repositories.InvestorSync
{
    public class InvestorSyncReopsitory : IInvestorSyncReopsitory
    {
        private readonly IOracleDbManagerAsync _dbManager;
        private readonly ICommonRepository _commonRepository;
        private readonly ICustomerRepository _customerRepository;
        public InvestorSyncReopsitory(IOracleDbManagerAsync dbManager, ICommonRepository commonRepository, ICustomerRepository customerRepository)
        {
            _dbManager = dbManager;
            _commonRepository = commonRepository;
            _customerRepository = customerRepository;
        }
        public async Task<List<InvestorSyncViewModel>> GetData(PaginationRequest<InvestorSyncFilters> req, CancellationToken cancellationToken)
        {
            string filtersObj = JsonSerializer.Serialize(req.filter);
            var parameters = new InvestorSyncFilters();
            parameters.memberCodeId = req.Filters.memberCodeId;

            List<InvestorSyncViewModel> _lst = new List<InvestorSyncViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            if (req.filter != null && !string.IsNullOrEmpty(req.filter.Trim()) && req.filter != "null")
            {
                FilterClass? filters = JsonSerializer.Deserialize<FilterClass>(req.filter);
                foreach (var item in filters.filters)
                {
                    switch (item.field.ToUpper())
                    {
                        case "CRM01_INVESTORID":
                            parameters.crm01_investorid = item.value;
                            break;

                        case "ISSYNC":
                            parameters.IsSync = item.value;
                            break;

                        default:
                            break;
                    }
                }
            }

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_48_Id", Value = parameters.memberCodeId, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_number", Value = req.PageNo, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_page_size", Value = req.PageSize, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_investor_id", Value = parameters.crm01_investorid, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "p_is_sync", Value = parameters.IsSync, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<InvestorSyncViewModel>("PKG_CRM01_INVESTORSYNC.GET_DATA", lstParams, "pview", cancellationToken);

            return _lst;
        }

        public async Task<InvestorSyncViewModel> ExportReqMessage(string messageId, CancellationToken cancellationToken)
        {
            List<InvestorSyncViewModel> _lst = new List<InvestorSyncViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "p_view", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_msg_id", Value = messageId, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<InvestorSyncViewModel>("PKG_CRM01_INVESTORSYNC.GET_SPECIFIC_MESSAGE", lstParams, "p_view", cancellationToken);

            return _lst.FirstOrDefault();
        }

        public async Task<InvestorSyncViewModel> ExportResMessage(string messageId, CancellationToken cancellationToken)
        {
            List<InvestorSyncViewModel> _lst = new List<InvestorSyncViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "p_view", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_msg_id", Value = messageId, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<InvestorSyncViewModel>("PKG_CRM01_INVESTORSYNC.GET_RES_MESSAGE", lstParams, "p_view", cancellationToken);

            return _lst.FirstOrDefault();
        }

        public byte[] ExportToExcel(List<InvestorSyncViewModel> data)
        {
            var result = data.Select(item => new InvestorSyncExportModel()
            {
                crm01_investorid = item.crm01_investorid,
                CRM01_LONGNAME = item.CRM01_LONGNAME,
                CRM01_FIRSTNAME = item.CRM01_FIRSTNAME,
                crm01_activationtype = item.crm01_activationtype,
                CRM01_STACKEHOLDERTYPE = item.CRM01_STACKEHOLDERTYPE,
                crm01_message = item.crm01_message,
                CRM01_ISSYNCED = item.CRM01_ISSYNCED == 1 ? "Yes" : "No",
                CRM01_IS_AVAIL_SYNC = item.CRM01_IS_AVAIL_SYNC == 1 ? "Yes" : "No",
                CRM01_SYNC_ID = item.CRM01_SYNC_ID,
                CRM01_REQ_MESSAGE = item.CRM01_REQ_MESSAGE

            }).ToList();

            byte[] fileBytes = _commonRepository.ExportToExcel(result);

            return fileBytes;
        }

        public async Task<string> GetEdaaMsg(string messageId, int memberCodeId, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            List<InvestorSyncViewModel> _lst = new List<InvestorSyncViewModel>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter { ParameterName = "p_view", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });
            lstParams.Add(new OracleParameter() { ParameterName = "p_msg_id", Value = messageId, Direction = System.Data.ParameterDirection.Input, });

            _lst = await _dbManager.GetStoredProcedureRefCursorAsync<InvestorSyncViewModel>("PKG_CRM01_INVESTORSYNC.Get_EDAA_Message", lstParams, "p_view", cancellationToken);
            InvestorSyncViewModel res = new InvestorSyncViewModel();
            res = _lst.FirstOrDefault();

            if (!string.IsNullOrEmpty(res.crm01_message))
            {
                var accountDetail = _commonRepository.GetBetween(res?.crm01_message, ":77E:", "-}");

                InvestorSyncControls controls = await GetInvestorControls(cancellationToken);

                var relatedReferenceId = _commonRepository.GetBetween(accountDetail, ":20C::RELA//", Environment.NewLine);
                accountDetail = accountDetail.Replace(":20C::RELA//" + relatedReferenceId, "");
                accountDetail = accountDetail.Replace(Environment.NewLine, "");

                var tokens = accountDetail.Split(';');

                CustomerBasicInfoReqModel customer = new CustomerBasicInfoReqModel();

                Dictionary<int, InterestedPartyViewModel> interestedPartyDictionary = new Dictionary<int, InterestedPartyViewModel>();
                Dictionary<int, StackeHolderReqModel> stakeholderIdentifiers = new Dictionary<int, StackeHolderReqModel>();
                Dictionary<int, AddressDetailsReqModel> addressDetails = new Dictionary<int, AddressDetailsReqModel>();
                Dictionary<int, ContactDetailsReqModel> contacts = new Dictionary<int, ContactDetailsReqModel>();

                foreach (var token in tokens)
                {
                    var temp = token.Split(new string[] { ":=" }, StringSplitOptions.None);
                    var tag = temp[0].Replace("\n", "").Replace("\r", "");
                    var interestedPartyTags = tag.Split('(');
                    var revenueTag = tag.Split('(');
                    var revenueTag1 = tag.Replace(".", "");

                    var value = "";

                    if (temp.Length > 1)
                        value = temp[1].ToString().Replace("\n", "").Replace("\r", "");

                    if (!string.IsNullOrEmpty(value))
                    {
                        switch (tag)
                        {
                            case "ActivationType":
                                customer.CRM01_ACTIVE = Convert.ToInt32(controls.ActivationState.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                break;
                            case "AdministratorCode":
                                customer.member_code = value;
                                break;
                            case "LongName":
                                customer.CRM01_LONG_NAME = value;
                                break;
                            case "FirstName":
                                customer.CRM01_FIRST_NAME = value;
                                break;
                            case "EnglishLongName":
                                customer.CRM01_ENG_LONG_NAME = value;
                                break;
                            case "EnglishShortName":
                                customer.CRM01_ENG_SHORT_NAME = value;
                                break;
                            case "TaxPayerIdentifier":
                                customer.CRM01_TAX_IDENTIFIER = value;
                                break;
                            case "RegistrationCountry":
                                customer.CRM01_REG_RF09_ID = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text == value)?.Code);
                                break;

                            case "TaxResidence":
                                customer.CRM01_TAX_RF09_ID = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text == value)?.Code);
                                break;
                            case "InterestedParty":
                                //customer.CRM01_INTERESTED_PARTY = value;
                                break;
                            case "StakeholderType":
                                customer.CRM01_STAKEHOLDER_TYPE = value.ToUpper() == "INDIVIDUAL"
                                    ? customer.CRM01_STAKEHOLDER_TYPE = "1"
                                    : customer.CRM01_STAKEHOLDER_TYPE = "2";
                                break;
                            case "PreferredLanguageOfCommunication":
                                customer.CRM01_COMM_LANGUAGE = Convert.ToInt32(controls.commonLanguageList.FirstOrDefault(x => x.text == value)?.Code); ;
                                break;
                            case "PreferredMethodOfCommunication":
                                customer.CRM01_COMM_METHOD = Convert.ToInt32(controls.commonMethodList.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code); ;
                                break;
                            case "BirthDate":
                                customer.CRM01_DOB = DateTime.ParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            case "CountryOfResidence":
                                customer.CRM01_RESIDENCE_RF09_ID = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text == value)?.Code);
                                break;
                            case "DeceasedDate":
                                customer.CRM01_DECEASED_DATE = DateTime.ParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            case "Citizenship":
                                customer.CRM01_CITIZEN_RF09_ID = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text == value)?.Code);
                                break;
                            case "InvestorCategory":
                                customer.CRM01_RF32_ID = Convert.ToInt32(controls.investorCategoryList.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                break;
                            case "SecondName":
                                customer.CRM01_SECOND_NAME = value;
                                break;
                            case "ThirdName":
                                customer.CRM01_THIRD_NAME = value;
                                break;
                            case "FourthName":
                                customer.CRM01_FOURTH_NAME = value;
                                break;
                            case "HjiriBirthDate":
                                customer.CRM01_HJIRI_DOB = value;
                                break;
                            case "Gender":
                                customer.CRM01_GENDER = Convert.ToInt32(controls.genderList.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                break;
                        }
                    }
                }

                customer.CRM01_EDAA_ID = res.crm01_investorid;
                customer.CRM01_RF48_ID = memberCodeId;

                CustomerBasicInfoReqModel basicInforResponse = await _customerRepository.SaveCustomerBasicInfo(customer, true, cancellationToken);

                if (basicInforResponse.CRM01_ID > 0)
                {
                    InvestorSyncViewModel investorSyncRequest = new InvestorSyncViewModel();

                    investorSyncRequest.crm01_messageid = messageId;
                    investorSyncRequest.crm01_activationtype = controls.ActivationState.FirstOrDefault(x => Convert.ToInt32(x.Code) == customer.CRM01_ACTIVE)?.text;
                    investorSyncRequest.CRM01_FIRSTNAME = customer.CRM01_FIRST_NAME;
                    investorSyncRequest.crm01_investorid = customer.CRM01_EDAA_ID;
                    investorSyncRequest.CRM01_LONGNAME = customer.CRM01_LONG_NAME;
                    investorSyncRequest.CRM01_STACKEHOLDERTYPE = controls.stakeholderTypeList.FirstOrDefault(x => x.Code == customer.CRM01_STAKEHOLDER_TYPE)?.text;
                    investorSyncRequest.CRM01_ISSYNCED = 1;

                    Update_Req_Data(investorSyncRequest, cancellationToken);

                    message = "Investor BasicInfo Sync Successfully";
                }

                List<DropDowns> stakeHolderData = await _commonRepository.GetStakeholderIdentify(Convert.ToInt32(customer.CRM01_STAKEHOLDER_TYPE), cancellationToken);

                foreach (var token in tokens)
                {
                    var temp = token.Split(new string[] { ":=" }, StringSplitOptions.None);
                    var tag = temp[0].Replace("\n", "").Replace("\r", "");
                    if (tag == "Comments")
                    {
                        break;
                    }
                    var stakeholderTags = tag.Split('(');

                    var value = "";

                    if (temp.Length > 1)
                        value = temp[1].ToString().Replace("\n", "").Replace("\r", "");

                    if (stakeholderTags.Length > 1 && !string.IsNullOrEmpty(value))
                    {
                        var arrayTag = tag.Remove(tag.Length - 3);
                        var ass = (tag.Substring(tag.Length - 2).Split(')'));
                        var arrayCount = Convert.ToInt32((tag.Substring(tag.Length - 2).Split(')'))[0]);

                        switch (arrayTag)
                        {
                            case "StakeholderIdentifier.Type":
                                if (!stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers.Add(arrayCount, new StackeHolderReqModel()
                                    {
                                        Type = value,
                                        crm02_rf23_type_id = Convert.ToInt32(stakeHolderData.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Id)
                                    });
                                }
                                else if (stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers[arrayCount].Type = value;
                                    stakeholderIdentifiers[arrayCount].crm02_rf23_type_id = Convert.ToInt32(stakeHolderData.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Id);
                                }
                                break;
                            case "StakeholderIdentifier.Identifier":
                                if (!stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers.Add(arrayCount, new StackeHolderReqModel()
                                    {
                                        crm02_number = value
                                    });
                                }
                                else if (stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers[arrayCount].crm02_number = value;
                                }
                                break;
                            case "StakeholderIdentifier.IssuingEntity":
                                if (!stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers.Add(arrayCount, new StackeHolderReqModel()
                                    {
                                        CRM02_ISSUING_ENTITY = value
                                    });
                                }
                                else if (stakeholderIdentifiers.ContainsKey(arrayCount))
                                {
                                    stakeholderIdentifiers[arrayCount].CRM02_ISSUING_ENTITY = value;
                                }
                                break;

                        }
                    }
                }

                if (stakeholderIdentifiers.Any())
                {
                    foreach (var stakeholder in stakeholderIdentifiers?.Values?.ToList())
                    {
                        if (stakeholder.Type.ToUpper() != "InvestorID".ToUpper() && !string.IsNullOrEmpty(stakeholder.crm02_number))
                        {
                            stakeholder.crm02_crm01_id = basicInforResponse.CRM01_ID;
                            var saveStakeholderResponse = _customerRepository.SaveStakeholder(stakeholder, cancellationToken);
                        }

                    }
                    message = "Investor Stakeholder Sync Successfully";
                }

                tokens = tokens.ToList().Select(s => s = s.Replace("\n", "")).ToArray();
                foreach (var token in tokens)
                {
                    var temp = token.Split(new string[] { ":=" }, StringSplitOptions.None);
                    var tag = temp[0].Replace("\n", "").Replace("\r", "");
                    if (tag == "Comments")
                    {
                        break;
                    }
                    var addressTags = tag.Split('(');

                    var value = "";

                    if (temp.Length > 1)
                        value = temp[1].ToString().Replace("\n", "").Replace("\r", "");

                    if (addressTags.Length > 1 && !string.IsNullOrEmpty(value))
                    {
                        var arrayTag = tag.Remove(tag.Length - 3);
                        var arrayCount = Convert.ToInt32((tag.Substring(tag.Length - 2).Split(')'))[0]);

                        switch (arrayTag)
                        {
                            case "AddressDetails.AddressLine1":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_address1 = value
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_address1 = value;
                                }
                                break;
                            case "AddressDetails.AddressLine2":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_address2 = value
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_address2 = value;
                                }
                                break;
                            case "AddressDetails.AddressLine3":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_address3 = value
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_address3 = value;
                                }
                                break;
                            case "AddressDetails.PostalCode":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_postalcode = value
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_postalcode = value;
                                }
                                break;



                            case "AddressDetails.City":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_rf10_id = Convert.ToInt32(controls.lstCities.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code)
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_rf10_id = Convert.ToInt32(controls.lstCities.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                }
                                break;



                            case "AddressDetails.StateProvince":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_stateprovince = value
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_stateprovince = value;
                                }
                                break;


                            case "AddressDetails.Country":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        country_code = value,
                                        crm04_rf09_id = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code)
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_rf09_id = Convert.ToInt32(controls.lstCountry.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                }
                                break;

                            case "AddressDetails.AddressType":
                                if (!addressDetails.ContainsKey(arrayCount))
                                {
                                    var asds = controls.addressTypes
                                        .FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code;
                                    addressDetails.Add(arrayCount, new AddressDetailsReqModel()
                                    {
                                        crm04_addresstype = Convert.ToInt32(controls.addressTypes.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code)
                                    });
                                }
                                else if (addressDetails.ContainsKey(arrayCount))
                                {
                                    addressDetails[arrayCount].crm04_addresstype = Convert.ToInt32(controls.addressTypes.FirstOrDefault(x => x.text.ToLower() == value.ToLower())?.Code);
                                }
                                break;
                        }
                    }
                }

                if (addressDetails.Any())
                {
                    foreach (var address in addressDetails?.Values?.ToList())
                    {
                        address.crm04_crm01_id = basicInforResponse.CRM01_ID;
                        var saveStakeholderResponse = _customerRepository.SaveAddressDetails(address, cancellationToken);
                    }
                    message = "Investor AddressDetails Sync Successfully";
                }

                tokens = tokens.ToList().Select(s => s = s.Replace("\n", "")).ToArray();
                foreach (var token in tokens)
                {
                    var temp = token.Split(new string[] { ":=" }, StringSplitOptions.None);
                    var tag = temp[0].Replace("\n", "").Replace("\r", "");
                    if (tag == "Comments")
                    {
                        break;
                    }
                    var contactsTags = tag.Split('(');

                    var value = "";

                    if (temp.Length > 1)
                        value = temp[1].ToString().Replace("\n", "").Replace("\r", "");

                    if (contactsTags.Length > 1 && !string.IsNullOrEmpty(value))
                    {
                        var arrayTag = tag.Remove(tag.Length - 3);
                        var arrayCount = Convert.ToInt32((tag.Substring(tag.Length - 2).Split(')'))[0]);

                        switch (arrayTag)
                        {
                            case "ContactDetails.ContactName":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_contact_name = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_contact_name = value;
                                }
                                break;
                            case "ContactDetails.Title":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_title = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_title = value;
                                }
                                break;
                            case "ContactDetails.PhoneNumber":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_home_no = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_home_no = value;
                                }
                                break;
                            case "ContactDetails.MobileNumber":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_mobile_no = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_mobile_no = value;
                                }
                                break;



                            case "ContactDetails.FaxNumber":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_fax_no = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_fax_no = value;
                                }
                                break;



                            case "ContactDetails.Email":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_email = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_email = value;
                                }
                                break;


                            case "ContactDetails.PublicDetails":
                                if (!contacts.ContainsKey(arrayCount))
                                {
                                    contacts.Add(arrayCount, new ContactDetailsReqModel()
                                    {
                                        crm03_public_detail = value
                                    });
                                }
                                else if (contacts.ContainsKey(arrayCount))
                                {
                                    contacts[arrayCount].crm03_public_detail = value;
                                }
                                break;
                        }
                    }
                }


                if (contacts.Any())
                {
                    foreach (var contact in contacts?.Values?.ToList())
                    {
                        contact.crm03_crm01_id = basicInforResponse.CRM01_ID;
                        var saveStakeholderResponse =_customerRepository.SaveContactDetails(contact,cancellationToken);
                    }

                    message = "Investor Synced Successfully";
                }

            }
            else
            {
                message = "Not available for synced";
            }
            return message;
        }


        private async Task<InvestorSyncControls> GetInvestorControls(CancellationToken cancellationToken)
        {
            List<DropDowns> dropDowns = new List<DropDowns>();
            List<OracleParameter> lstParams = new List<OracleParameter>();
            InvestorSyncControls customerControls = new InvestorSyncControls();

            lstParams.Add(new OracleParameter { ParameterName = "pview", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

            dropDowns = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("PKG_CRM01_INVESTORSYNC.Get_Acc_Dropdown", lstParams, "pview", cancellationToken);

            customerControls.accTypes = dropDowns.Where(x => x.Types == "acc types").ToList();
            customerControls.acclabels = dropDowns.Where(x => x.Types == "acc labels").ToList();
            customerControls.ninTypes = dropDowns.Where(x => x.Types == "nin types").ToList();
            customerControls.lstCountry = dropDowns.Where(x => x.Types == "country").ToList();
            customerControls.administratorCodeList = dropDowns.Where(x => x.Types == "Admin Code").ToList();
            customerControls.ActivationState = dropDowns.Where(x => x.Types == "activeType").ToList();
            customerControls.customersList = dropDowns.Where(x => x.Types == "AllCustomers").ToList();
            customerControls.Rf09 = dropDowns.Where(x => x.Types == "TaxRF09").ToList();
            customerControls.EdaaStatus = dropDowns.Where(x => x.Types == "EdaaStatus").ToList();
            customerControls.commonLanguageList = dropDowns.Where(x => x.Types == "preferLanguage").ToList();
            customerControls.investorCategoryList = dropDowns.Where(x => x.Types == "InvestorCategory").ToList();
            customerControls.genderList = dropDowns.Where(x => x.Types == "Gender").ToList();
            customerControls.addressTypes = dropDowns.Where(x => x.Types == "AddressType").ToList();
            customerControls.lstCities = dropDowns.Where(x => x.Types == "Cities").ToList();
            customerControls.lstIdentificationCountries = dropDowns.Where(x => x.Types == "countryCode").ToList();
            customerControls.stakeholderTypeList = dropDowns.Where(x => x.Types == "SHType").ToList();
            customerControls.commonMethodList = dropDowns.Where(x => x.Types == "PrefMethod").ToList();

            return customerControls;
        }

        private async Task Update_Req_Data(InvestorSyncViewModel req, CancellationToken cancellationToken)
        {
            List<DropDowns> dropDowns = new List<DropDowns>();
            List<OracleParameter> lstParams = new List<OracleParameter>();

            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_messageid", Value = req.crm01_messageid, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_investorid", Value = req.crm01_investorid, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_activationtype", Value = req.crm01_activationtype, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_longname", Value = req.CRM01_LONGNAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_firstname", Value = req.CRM01_FIRSTNAME, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_stackeholdertype", Value = req.CRM01_STACKEHOLDERTYPE, Direction = System.Data.ParameterDirection.Input, });
            lstParams.Add(new OracleParameter() { ParameterName = "Pcrm01_issynced", Value = req.CRM01_ISSYNCED, Direction = System.Data.ParameterDirection.Input, });

            StoredProcedureResult result = await _dbManager.ExecuteStoredProcedureWithOutputAsync("PKG_CRM01_INVESTORSYNC.UPDATE_REQ_DATA", lstParams, cancellationToken);

        }
    }
}
