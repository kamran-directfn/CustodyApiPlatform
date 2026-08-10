using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Symbol;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Symbol;
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
    [Route("api/v{version:apiVersion}/symbol")]
    [ApiController]
    public class SymbolController : CustodyControllerBase
    {
        private readonly ISymbolRepository _symbolRepository;
        private readonly ICustodyUserContext _custodyUserContext;

        public SymbolController(ISymbolRepository symbolRepository,ICustodyUserContext custodyUserContext)
        {
            _symbolRepository = symbolRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_SYMBOLS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<SymbolViewModel> data = await _symbolRepository.GetAllSymbolsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SYMBOL_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById(int symbolId,CancellationToken cancellationToken)
        {
            SymbolViewModel data = await _symbolRepository.GetSymbolById(symbolId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_SYMBOL")]
        [HttpPost("approve")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<SymbolViewModel> data = await _symbolRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_SYMBOL")]
        [HttpPost("pending")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            List<SymbolViewModel> data = await _symbolRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_SYMBOL")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _symbolRepository.DeleteSymbol(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_SYMBOL")]
        [HttpPost("save")]
        public async Task<IActionResult> Add(SymbolReqModel request, CancellationToken cancellationToken)
        {
            request.RF02_CREATED_BY = (int)_custodyUserContext.UserId;
            SymbolReqModel data = await _symbolRepository.AddSymbol(request, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_SYMBOL")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(SymbolReqModel request, CancellationToken cancellationToken)
        {
            request.RF02_MODIFIED_BY = (int)_custodyUserContext.UserId;
            SymbolReqModel data = await _symbolRepository.UpdateSymbol(request, cancellationToken);

            return Success(data);
        }
    }
}