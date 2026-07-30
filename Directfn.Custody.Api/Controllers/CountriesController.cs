using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Countries;
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
    [Route("api/v{version:apiVersion}/country")]
    [ApiController]

    public class CountriesController : CustodyControllerBase
    {
        private readonly ICountriesRepository _countriesRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public CountriesController(ICountriesRepository countriesRepository, ICustodyUserContext custodyUserContext)
        {
            _countriesRepository = countriesRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_COUNTRIES")]
        [HttpGet("get-countries")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<CountriesViewModel> data = await _countriesRepository.GetAllCountriesAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_COUNTRY_BY_ID")]
        [HttpGet("get-country-by-id")]
        public async Task<IActionResult> GetById(int countryId, CancellationToken cancellationToken)
        {
            CountriesViewModel data = await _countriesRepository.GetCountryById(countryId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_COUNTRY")]
        [HttpPost("approve-country")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CountriesViewModel> data = await _countriesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_COUNTRY")]
        [HttpPost("pending-country")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CountriesViewModel> data = await _countriesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_COUNTRY")]
        [HttpPost("delete-country")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _countriesRepository.DeleteCountry(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_COUNTRY")]
        [HttpPost("save-country")]
        public async Task<IActionResult> Add(CountriesReqModel request, CancellationToken cancellationToken)
        {
            request.RF09_CREATED_BY = (int)_custodyUserContext.UserId;
            CountriesReqModel data = await _countriesRepository.SaveCountry(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_COUNTRY")]
        [HttpPost("update-country")]
        public async Task<IActionResult> Update(CountriesReqModel request, CancellationToken cancellationToken)
        {
            request.RF09_MODIFIED_BY = (int)_custodyUserContext.UserId;
            CountriesReqModel data = await _countriesRepository.UpdateCountry(request, cancellationToken);

            return Success(data);
        }
    }
}
