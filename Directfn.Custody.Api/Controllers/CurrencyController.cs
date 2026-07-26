using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Currency;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Currency;
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
    [Route("api/v{version:apiVersion}/currency")]
    [ApiController]
    public class CurrencyController : CustodyControllerBase
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public CurrencyController(ICurrencyRepository currencyRepository, ICustodyUserContext custodyUserContext)
        {
            _currencyRepository = currencyRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_CURRENCIES")]
        [HttpGet("get-currencies")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<CurrencyViewModel> data = await _currencyRepository.GetAllCurrenciesAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_CURRENCY_BY_ID")]
        [HttpGet("get-currency-by-id")]
        public async Task<IActionResult> GetById(int currencyId, CancellationToken cancellationToken)
        {
            CurrencyViewModel data = await _currencyRepository.GetCurrencyById(currencyId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_CURRENCY")]
        [HttpPost("approve-currency")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CurrencyViewModel> data = await _currencyRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_CURRENCY")]
        [HttpPost("pending-currency")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<CurrencyViewModel> data = await _currencyRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_CURRENCY")]
        [HttpPost("delete-currency")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _currencyRepository.DeleteCurrency(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_CURRENCY")]
        [HttpPost("save-currency")]
        public async Task<IActionResult> Add(CurrencyReqModel request, CancellationToken cancellationToken)
        {
            request.RF08_CREATED_BY = (int)_custodyUserContext.UserId;
            var data = await _currencyRepository.AddCurrency(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_CURRENCY")]
        [HttpPost("update-currency")]
        public async Task<IActionResult> Update(CurrencyReqModel request, CancellationToken cancellationToken)
        {
            request.RF08_MODIFIED_BY = (int)_custodyUserContext.UserId;
            var data = await _currencyRepository.UpdateCurrency(request, cancellationToken);

            return Success(data);
        }
    }
}
