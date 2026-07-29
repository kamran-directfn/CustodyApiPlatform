using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Cities;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Cities;
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
    [Route("api/v{version:apiVersion}/city")]
    [ApiController]
    public class CitiesController : CustodyControllerBase
    {
        private readonly ICitiesRepository _citiesRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public CitiesController(ICitiesRepository citiesRepository,ICustodyUserContext custodyUserContext)
        {
            _citiesRepository = citiesRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_CITIES")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<CitiesViewModel> data = await _citiesRepository.GetAllCitiesAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_CITY_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int cityId, CancellationToken cancellationToken)
        {
            CitiesViewModel data = await _citiesRepository.GetCityById(cityId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_CITY")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CitiesViewModel> data = await _citiesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_CITY")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CitiesViewModel> data = await _citiesRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_CITY")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _citiesRepository.DeleteCity(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_CITY")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(CitiesReqModel request, CancellationToken cancellationToken)
        {
            request.RF10_CREATED_BY = (int)_custodyUserContext.UserId;
            CitiesReqModel data = await _citiesRepository.SaveCity(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_CITY")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(CitiesReqModel request, CancellationToken cancellationToken)
        {
            request.RF10_MODIFIED_BY = (int)_custodyUserContext.UserId;
            CitiesReqModel data = await _citiesRepository.UpdateCity(request, cancellationToken);

            return Success(data);
        }

    }
}
