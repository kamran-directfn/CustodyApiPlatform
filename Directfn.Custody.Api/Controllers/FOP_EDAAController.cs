using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.FOP;
using Directfn.Custody.ApiFramework.Repositories.FOP_EDAA;
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
    [Route("api/v{version:apiVersion}/fop")]
    [ApiController]

    public class FOP_EDAAController : CustodyControllerBase
    {
        private readonly IFOP_EDAARepository _fOP_EDAARepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public FOP_EDAAController(IFOP_EDAARepository fOP_EDAARepository, ICommonRepository commonRepository, ICustodyUserContext custodyUserContext)
        {
            _fOP_EDAARepository = fOP_EDAARepository;
            _commonRepository = commonRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_FOP_EDAA")]
        [HttpPost("get")]
        public async Task<IActionResult> Get(PaginationRequest req, CancellationToken cancellationToken)
        {
            int rf48_id = (int)_custodyUserContext.MemberCodeId;
            int created_by = (int)_custodyUserContext.UserId;
            int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

            List<FOPValidate> data = await _fOP_EDAARepository.GetAllFOP_EDAA_Async(req, rf48_id, portfolioId, cancellationToken);

            return Success(data);
        }
    }
}
