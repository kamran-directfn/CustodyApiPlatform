using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Cities;
using Directfn.Custody.ApiFramework.Common.DTOs.Customer;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Customer;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/customer")]
    [ApiController]
    public class CustomerController : CustodyControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public CustomerController(ICustomerRepository customerRepository, ICustodyUserContext custodyUserContext)
        {
            _customerRepository = customerRepository;
            _custodyUserContext = custodyUserContext;
        }
        #region basicInfo
        [AuditAction("GET_CUSTOMERS")]
        [HttpPost("get-customers-basic-info")]
        public async Task<IActionResult> GetCustomerBasicInfoList(PaginationRequest<CustomerBasicInfoFilter> req, CancellationToken cancellationToken)
        {
            req.Filters.memberCodeId = (int)_custodyUserContext.MemberCodeId;
            req.Filters.PortfolioGroupId = (int)_custodyUserContext.PortfolioGroupId;

            List<CustomerBasicInfoViewModel> data = await _customerRepository.GetAllCustomers(req, cancellationToken);

            int totalCount = data?.FirstOrDefault()?.TotalRecords ?? 0;

            return Success(new { data, totalCount });
        }

        [AuditAction("GET_CUSTOMER_BY_ID")]
        [HttpGet("get-customer-by-id")]
        public async Task<IActionResult> GetCustomerById(int Id, CancellationToken cancellationToken)
        {
            int memberCodeId = (int)_custodyUserContext.MemberCodeId;

            CustomerBasicInfoViewModel data = await _customerRepository.GetCustomerById(Id, memberCodeId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_Customer")]
        [HttpPost("approve-customer")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.CustomerPostUnpostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_BANK")]
        [HttpPost("pending-customer")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.CustomerPostUnpostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }


        [AuditAction("DELETE_Customer")]
        [HttpPost("delete-customer")]
        public async Task<IActionResult> DeleteCustomer(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.DeleteCustomer(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("ADD_CUSTOMER_BASIC_INFO")]
        [HttpPost("add-customer-basic-info")]
        public async Task<IActionResult> AddCustomerBasicInfo(CustomerBasicInfoReqModel req, CancellationToken cancellationToken)
        {
            req.CRM01_RF48_ID = (int)_custodyUserContext.MemberCodeId;
            req.CRM01_CREATED_BY = (int)_custodyUserContext.UserId;

            CustomerBasicInfoReqModel data = await _customerRepository.SaveCustomerBasicInfo(req, false, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_CUSTOMER_BASIC_INFO")]
        [HttpPost("update-customer-basic-info")]
        public async Task<IActionResult> UpdateCustomerBasicInfo(CustomerBasicInfoReqModel req, CancellationToken cancellationToken)
        {
            req.CRM01_RF48_ID = (int)_custodyUserContext.MemberCodeId;
            req.CRM01_MODIFIED_BY = (int)_custodyUserContext.UserId;

            CustomerBasicInfoReqModel data = await _customerRepository.SaveCustomerBasicInfo(req, false, cancellationToken);

            return Success(data);
        }
        #endregion basicInfo

        #region stackeHolder
        [AuditAction("GET_STAKE_HOLDER")]
        [HttpGet("get-stake-holders")]
        public async Task<IActionResult> GetStakeholderList(int Id, CancellationToken cancellationToken)
        {
            List<StackeHolderViewModel> data = await _customerRepository.GetStakeholderList(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_STAKE_BY_ID")]
        [HttpGet("get-stake-by-id")]
        public async Task<IActionResult> GetStakeById(int Id, CancellationToken cancellationToken)
        {
            StackeHolderViewModel data = await _customerRepository.GetStakeById(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_STAKE_HOLDER")]
        [HttpPost("delete-stake-holder")]
        public async Task<IActionResult> DeleteStakeholder(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.DeleteStakeholder(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_STAKE_HOLDER")]
        [HttpPost("save-stake-holder")]
        public async Task<IActionResult> SaveStakeholder(StackeHolderReqModel req, CancellationToken cancellationToken)
        {
            req.crm02_created_by = (int)_custodyUserContext.UserId;
            StackeHolderReqModel data = await _customerRepository.SaveStakeholder(req, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_STAKE_HOLDER")]
        [HttpPost("update-stake-holder")]
        public async Task<IActionResult> UpdateStakeholder(StackeHolderReqModel req, CancellationToken cancellationToken)
        {
            req.crm02_created_by = (int)_custodyUserContext.UserId;
            StackeHolderReqModel data = await _customerRepository.SaveStakeholder(req, cancellationToken);

            return Success(data);
        }

        #endregion stackeHolder

        #region AddressDetails

        [AuditAction("GET_ADDRESS_DETAILS")]
        [HttpGet("get-address-details")]
        public async Task<IActionResult> GetAddressDetailsList(int Id, CancellationToken cancellationToken)
        {
            List<AddressDetailsViewModel> data = await _customerRepository.GetAddressDetailsList(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET-ADDRESS-BY-ID")]
        [HttpGet("get-address-by-id")]
        public async Task<IActionResult> GetAddressById(int Id, CancellationToken cancellationToken)
        {
            AddressDetailsViewModel data = await _customerRepository.GetAddressById(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_ADDRESS_DETAILS")]
        [HttpPost("delete-address-details")]
        public async Task<IActionResult> DeleteAddressDetails(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.DeleteAddressDetails(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_ADDRESS_DETAILS")]
        [HttpPost("save-address-details")]
        public async Task<IActionResult> AddAddressDetails(AddressDetailsReqModel req, CancellationToken cancellationToken)
        {
            req.crm04_created_by = (int)_custodyUserContext.UserId;
            AddressDetailsReqModel data = await _customerRepository.SaveAddressDetails(req, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_ADDRESS_DETAILS")]
        [HttpPost("update-address-details")]
        public async Task<IActionResult> UpdateAddressDetails(AddressDetailsReqModel req, CancellationToken cancellationToken)
        {
            req.crm04_created_by = (int)_custodyUserContext.UserId;
            AddressDetailsReqModel data = await _customerRepository.SaveAddressDetails(req, cancellationToken);

            return Success(data);
        }

        #endregion AddressDetails
        #region ContactDetails
        [AuditAction("GET_CONTACT_DETAILS")]
        [HttpGet("get-contact-details")]
        public async Task<IActionResult> GetContactDetailsList(int Id, CancellationToken cancellationToken)
        {
            List<ContactDetailsViewModel> data = await _customerRepository.GetContactDetailsList(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_CONTACT_DETAILS_BY_ID")]
        [HttpGet("get-contact-details-by-id")]
        public async Task<IActionResult> GetContactDetailsById(int Id, CancellationToken cancellationToken)
        {
            ContactDetailsViewModel data = await _customerRepository.GetContactDetailsById(Id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_CONTACT_DETAILS")]
        [HttpPost("delete-contact-details")]
        public async Task<IActionResult> DeleteContactDetails(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _customerRepository.DeleteContactDetails(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_CONTACT_DETAILS")]
        [HttpPost("save-contact-details")]
        public async Task<IActionResult> AddContactDetails(ContactDetailsReqModel req, CancellationToken cancellationToken)
        {
            req.crm03_created_by = (int)_custodyUserContext.UserId;
            ContactDetailsReqModel data = await _customerRepository.SaveContactDetails(req, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_CONTACT_DETAILS")]
        [HttpPost("update-contact-details")]
        public async Task<IActionResult> UpdateContactDetails(ContactDetailsReqModel req, CancellationToken cancellationToken)
        {
            req.crm03_created_by = (int)_custodyUserContext.UserId;
            ContactDetailsReqModel data = await _customerRepository.SaveContactDetails(req, cancellationToken);

            return Success(data);
        }
        #endregion ContactDetails
    }
}
