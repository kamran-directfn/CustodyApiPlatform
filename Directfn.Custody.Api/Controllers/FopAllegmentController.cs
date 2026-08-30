using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.FOPAllegment;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.FOP_Allegment;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using System.Text;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fop-allegment")]
    [ApiController]
    public class FopAllegmentController : CustodyControllerBase
    {
        private readonly IFOPAllegmentRepository _fOPAllegmentRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        private readonly ICommonRepository _commonRepository;
        public FopAllegmentController(IFOPAllegmentRepository fOPAllegmentRepository, ICustodyUserContext custodyUserContext, ICommonRepository commonRepository)
        {
            _fOPAllegmentRepository = fOPAllegmentRepository;
            _custodyUserContext = custodyUserContext;
            _commonRepository = commonRepository;
        }

        [AuditAction("GET_FOP_ALLEGMENT")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(string date, string? recAgnt, string? delAgnt,CancellationToken cancellationToken)
        {
            int rf48_id = (int)_custodyUserContext.MemberCodeId;
            int portfolioId = (int)_custodyUserContext.PortfolioGroupId;

            List<FOPAllegmentViewModel> data = await _fOPAllegmentRepository.GetFopAllegmentAsync(date, recAgnt, delAgnt, rf48_id, portfolioId, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_FOP_ALLEGMENT-CHILD")]
        [HttpGet("get-fop-allegment-child")]
        public async Task<IActionResult> GetFopAllegmentChild(string referenceNo, CancellationToken cancellationToken)
        {
            List<FOPAllegmentViewModel> data = await _fOPAllegmentRepository.GetFopAllegmentChild(referenceNo, cancellationToken);

            return Success(data);
        }

        [AuditAction("EXPORT_MT578_MSG")]
        [HttpGet("export-mt578-msg")]
        public async Task<IActionResult> ExportMT578Msg(int id, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            FOPAllegmentViewModel data = await _fOPAllegmentRepository.ExportMT578Msg(id, cancellationToken);

            if (data != null)
            {
                writer.WriteLine(data.PRS62_RAW_MSG);
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"I578_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File(memoryStream.ToArray(), "text/plain", fileName);
        }

        [AuditAction("EXPORT_MT578_CANCEL_MSG")]
        [HttpGet("export-mt578-cancel-msg")]
        public async Task<IActionResult> DownloadCancelledMessage(int id, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            FOPAllegmentViewModel data = await _fOPAllegmentRepository.ExportMT578Msg(id, cancellationToken);

            if (data != null)
            {
                writer.WriteLine(data.PRS62_CANC_MSG);
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"I578_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File(memoryStream.ToArray(), "text/plain", fileName);
        }

        [AuditAction("EXPORT_MT598_MSG")]
        [HttpGet("export-mt598-msg")]
        public async Task<IActionResult> ExportMessageNested(int id, string referenceNo, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            List<FOPAllegmentViewModel> data = await _fOPAllegmentRepository.GetFopAllegmentChild(referenceNo, cancellationToken);

            if (data != null)
            {
                writer.WriteLine(data.FirstOrDefault(x => x.prs62ID == id)?.PRS62_RAW_MSG ?? "");
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"I598_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File(memoryStream.ToArray(), "text/plain", fileName);
        }
    }
}
