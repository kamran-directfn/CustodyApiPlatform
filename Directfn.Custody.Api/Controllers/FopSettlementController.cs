using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPSettlement;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.FOP_Settlement;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Data;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fop-settlement")]
    [ApiController]
    public class FopSettlementController : CustodyControllerBase
    {
        private readonly IFOPSettlementRepository _fOPSettlementRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        private readonly ICommonRepository _commonRepository;
        public FopSettlementController(IFOPSettlementRepository fOPSettlement, ICustodyUserContext custodyUserContext, ICommonRepository commonRepository)
        {
            _fOPSettlementRepository = fOPSettlement;
            _custodyUserContext = custodyUserContext;
            _commonRepository = commonRepository;
        }

        [AuditAction("GET_FOP_SETTLEMENT")]
        [HttpPost("get")]
        public async Task<IActionResult> Get(PaginationRequest<FOPSettlementFilter> req, CancellationToken cancellationToken)
        {
            req.Filters.Rf48Id = (int)_custodyUserContext.MemberCodeId;
            req.Filters.PortfolioGroupId = (int)_custodyUserContext.PortfolioGroupId;

            List<FOPSettlementViewModel> data = await _fOPSettlementRepository.GetFopSettlementAsync(req, cancellationToken);

            int totalCount = data?.FirstOrDefault()?.totalCount ?? 0;

            return Success(new { data, totalCount });
        }

        [AuditAction("EXPORT_FOP_SETTLEMENT")]
        [HttpPost("export")]
        public async Task<IActionResult> Export(PaginationRequest<FOPSettlementFilter> req, CancellationToken cancellationToken)
        {
            req.Filters.Rf48Id =  (int)_custodyUserContext.MemberCodeId;
            req.Filters.PortfolioGroupId =  (int)_custodyUserContext.PortfolioGroupId;

            List<FOPSettlementViewModel> data = await _fOPSettlementRepository.GetFopSettlementAsync(req, cancellationToken);

            byte[] fileBytes = _fOPSettlementRepository.ExportToExcel(data);

            string fileName = $"FOP_Settlement_Export_{_commonRepository.GetReqId()}.xlsx";

            return File(fileBytes,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",fileName);
        }
      
    }
}
