using Directfn.Custody.ApiFramework.Common.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync
{
    public class InvestorSyncControls
    {
        public List<DropDowns> lstBranch { get; set; }
        public List<DropDowns> lstAttorney { get; set; }
        public List<DropDowns> lstRm { get; set; }
        public List<DropDowns> lstIdentificationTypes { get; set; }
        public List<DropDowns> lstIdentificationCountries { get; set; }
        public List<StackeHolderViewModel> lstIdentifications { get; set; }
        public List<DropDowns> lstCustomerType { get; set; }
        public List<DropDowns> lstCustomerGroups { get; set; }
        public List<DropDowns> lstCustodianBranches { get; set; }
        public List<DropDowns> lstBanks { get; set; }
        public List<DropDowns> lstMarkets { get; set; }
        public List<DropDowns> lsBrokers { get; set; }
        public List<DropDowns> lstCountry { get; set; }
        public List<DropDowns> EdaaStatus { get; set; }
        public List<DropDowns> lstCities { get; set; }
        public List<DropDowns> lstCitiesCache { get; set; }
        public List<DropDowns> lstCustomerNin { get; set; }
        public List<DropDowns> lstCustomerAddress { get; set; }
        public List<DropDowns> lstMasterClients { get; set; }
        public List<DropDowns> lstAccountType { get; set; }
        public List<DropDowns> lstBankAccountType { get; set; }
        public List<DropDowns> lstCurrencies { get; set; }
        public List<DropDowns> lstBankAccPrefix { get; set; }
        public List<DropDowns> activeTypeList { get; set; }
        public List<DropDowns> administratorCodeList { get; set; }
        public List<DropDowns> investorCategoryList { get; set; }
        public List<DropDowns> commonLanguageList { get; set; }
        public List<DropDowns> commonMethodList { get; set; }

        public List<DropDowns> interestedPartyList { get; set; }
        public List<DropDowns> interestedPartyOrganizationList { get; set; }

        public List<DropDowns> PaymentInstructionType { get; set; }
        public List<DropDowns> stakeholderTypeList { get; set; }
        public List<DropDowns> legalStatusList { get; set; }
        public List<DropDowns> genderList { get; set; }
        public List<DropDowns> addressTypes { get; set; }
        public List<DropDowns> priority { get; set; }
        public List<DropDowns> holdStatus { get; set; }
        public List<DropDowns> ninTypes { get; set; }
        public List<DropDowns> acclabels { get; set; }
        public List<DropDowns> accTypes { get; set; }
        public List<DropDowns> customersList { get; set; }
        public List<DropDowns> Rf09 { get; set; }
        //
        public List<DropDowns> IsinList { get; set; }
        public List<DropDowns> SettlementISONotification { get; set; }
        public List<DropDowns> CorporateActionISONotification { get; set; }
        public List<DropDowns> CentralCounterPartyISONotification { get; set; }

        public List<DropDowns> TradeTypes { get; set; }
        public List<DropDowns> TransferTypes { get; set; }
        public List<DropDowns> ActivationState { get; set; }

        public List<DropDowns> CA_References { get; set; }

        public List<DropDowns> SubscribeAsList { get; set; }
        public List<DropDowns> BicCodes { get; set; }
    }
}
