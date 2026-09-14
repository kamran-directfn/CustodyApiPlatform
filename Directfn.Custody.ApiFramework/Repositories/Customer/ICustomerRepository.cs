using Directfn.Custody.ApiFramework.Common.DTOs.Customer;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task<List<CustomerBasicInfoViewModel>> GetAllCustomers(PaginationRequest<CustomerBasicInfoFilter> req, CancellationToken cancellationToken);
        Task<CustomerBasicInfoViewModel> GetCustomerById(int Id, int memberCodeId, CancellationToken cancellationToken);
        Task<string> CustomerPostUnpostStatus(int crm01_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<string> DeleteCustomer(int crm01_id, int user_id, CancellationToken cancellationToken);
        Task<CustomerBasicInfoReqModel> SaveCustomerBasicInfo(CustomerBasicInfoReqModel req, bool isSync, CancellationToken cancellationToken);

        Task<List<StackeHolderViewModel>> GetStakeholderList(int Id, CancellationToken cancellationToken);
        Task<StackeHolderViewModel> GetStakeById(int Id, CancellationToken cancellationToken);
        Task<string> DeleteStakeholder(int crm02_id, int user_id, CancellationToken cancellationToken);
        Task<StackeHolderReqModel> SaveStakeholder(StackeHolderReqModel req, CancellationToken cancellationToken);

        Task<List<AddressDetailsViewModel>> GetAddressDetailsList(int Id, CancellationToken cancellationToken);
        Task<AddressDetailsViewModel> GetAddressById(int Id, CancellationToken cancellationToken);
        Task<string> DeleteAddressDetails(int crm04_id, int user_id, CancellationToken cancellationToken);
        Task<AddressDetailsReqModel> SaveAddressDetails(AddressDetailsReqModel req, CancellationToken cancellationToken);
        #region ContactDetails
        Task<List<ContactDetailsViewModel>> GetContactDetailsList(int Id, CancellationToken cancellationToken);
        Task<ContactDetailsViewModel> GetContactDetailsById(int Id, CancellationToken cancellationToken);
        Task<string> DeleteContactDetails(int crm03_id, int user_id, CancellationToken cancellationToken);
        Task<ContactDetailsReqModel> SaveContactDetails(ContactDetailsReqModel req, CancellationToken cancellationToken);
        #endregion ContactDetails
    }
}
