using Asp.Versioning;
using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.FOP;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fop")]
    [ApiController]
    public class FOPController : CustodyControllerBase
    {
        private readonly IFOPRepository _fOPRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICustodyUserContext _custodyUserContext;

        public FOPController(IFOPRepository fOPRepository, ICommonRepository commonRepository, ICustodyUserContext custodyUserContext)
        {
            _fOPRepository = fOPRepository;
            _commonRepository = commonRepository;
            _custodyUserContext = custodyUserContext;
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> Upload_ExcelFile(IFormFile file, CancellationToken cancellationToken)
        {
            int rf48_id = (int)_custodyUserContext.MemberCodeId;
            int created_by = (int)_custodyUserContext.UserId;
            int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

            List <FOPValidate> listFOP = new List<FOPValidate>();

            DataSet result = _commonRepository.UploadFileDataSet(file);
            if (result != null && result.Tables != null && result.Tables[0].Rows.Count > 0)
            {
                listFOP = await _fOPRepository.GetFOP(result, rf48_id, portfolioId, created_by, cancellationToken);
                listFOP = await _fOPRepository.DistinctUniqueRef(listFOP, cancellationToken);
                int batch_id = await _commonRepository.GetBatchID("FOP_Share_Movement", rf48_id, cancellationToken);
                listFOP = await _fOPRepository.FOPStagingTable(listFOP, batch_id, rf48_id, portfolioId, created_by, cancellationToken);


            }

            return Success(listFOP);
        }

        [HttpPost("save-fop-excel")]
        public async Task<IActionResult> SaveFOPExcel([FromRoute] int batchId, CancellationToken cancellation)
        {
            string MemberCode = _custodyUserContext.MemberCode;
            List<FOPValidate> list = await _fOPRepository.GetValidFOPByBatchId(batchId, cancellation);
            string message = await _fOPRepository.SaveFOP(list, MemberCode, cancellation);
            return Success(message);
        }
    }
}
