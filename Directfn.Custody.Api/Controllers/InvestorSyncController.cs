using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.InvestorSync;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.InvestorSync;
using Directfn.Custody.ApiFramework.Security;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Directfn.Custody.Api.Controllers
{
#if !DEBUG
[Authorize]
#endif
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/investor-sync")]
    [ApiController]
    public class InvestorSyncController : CustodyControllerBase
    {
        private IInvestorSyncReopsitory _investorSyncReopsitory;
        private readonly ICustodyUserContext _custodyUserContext;
        private readonly ICommonRepository _commonRepository;
        public InvestorSyncController(IInvestorSyncReopsitory investorSyncReopsitory, ICustodyUserContext custodyUserContext, ICommonRepository commonRepository)
        {
            _investorSyncReopsitory = investorSyncReopsitory;
            _custodyUserContext = custodyUserContext;
            _commonRepository = commonRepository;
        }

        [AuditAction("GET_SYNC_INVESTOR_DATA")]
        [HttpPost("get")]
        public async Task<IActionResult> GetData(PaginationRequest<InvestorSyncFilters> req, CancellationToken cancellationToken)
        {
            req.Filters.memberCodeId = (int)_custodyUserContext.MemberCodeId;

            List<InvestorSyncViewModel> data = await _investorSyncReopsitory.GetData(req, cancellationToken);

            int totalCount = data?.FirstOrDefault()?.TotalRecords ?? 0;

            return Success(new { data, totalCount });
        }

        [AuditAction("EXPORT_REQ_MESSAGE_O598")]
        [HttpGet("export-req-message-O598")]
        public async Task<IActionResult> ExportReqMessage(string id, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            InvestorSyncViewModel data = await _investorSyncReopsitory.ExportReqMessage(id, cancellationToken);

            if (!string.IsNullOrEmpty(data.CRM01_REQ_MESSAGE))
            {
                writer.WriteLine(data.CRM01_REQ_MESSAGE);
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"O598_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File(memoryStream.ToArray(), "text/plain", fileName);
        }

        [AuditAction("EXPORT_RES_MESSAGE_I598")]
        [HttpGet("export-res-message-I598")]
        public async Task<IActionResult> ExportResMessage(string id, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            InvestorSyncViewModel data = await _investorSyncReopsitory.ExportResMessage(id, cancellationToken);

            if (!string.IsNullOrEmpty(data.CRM01_REQ_MESSAGE))
            {
                writer.WriteLine(data.CRM01_REQ_MESSAGE);
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"I598_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File(memoryStream.ToArray(), "text/plain", fileName);
        }

        [AuditAction("EXPORT_INVESTOR_SYNC")]
        [HttpPost("export")]
        public async Task<IActionResult> Export(PaginationRequest<InvestorSyncFilters> req, CancellationToken cancellationToken)
        {
            req.Filters.memberCodeId = (int)_custodyUserContext.MemberCodeId;

            List<InvestorSyncViewModel> data = await _investorSyncReopsitory.GetData(req, cancellationToken);

            byte[] fileBytes = _investorSyncReopsitory.ExportToExcel(data);

            string fileName = $"Investor_Sync_Export_{_commonRepository.GetDateTimeForExportMsgs()}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [AuditAction("Investor_Sync_With_Edaa")]
        [HttpPost("sync-with-edaa")]
        public async Task<IActionResult> SyncWithEdaa(string[] messagesIds, CancellationToken cancellationToken)
        {
            string data = await _investorSyncReopsitory.GetEdaaMsg(messagesIds[0], (int)_custodyUserContext.MemberCodeId, cancellationToken);

            return Success(data);
        }

    }
}