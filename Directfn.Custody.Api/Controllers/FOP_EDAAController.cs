using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using Directfn.Custody.ApiFramework.Common.DTOs.FOP;
using Directfn.Custody.ApiFramework.Common.DTOs.Pagination;
using Directfn.Custody.ApiFramework.Common.Enumerations;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.FOP;
using Directfn.Custody.ApiFramework.Repositories.FOP_EDAA;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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

            int totalCount = data?.FirstOrDefault()?.totalCount ?? 0;

            return Success(new { data, totalCount });
        }

        [AuditAction("GET_FOP_EDAA-CHILD")]
        [HttpGet("get-fop-edaa-child")]
        public async Task<IActionResult> GetFopEddaChild(string referenceNo, CancellationToken cancellationToken)
        {
            List<FOP_Child_Data> data = await _fOP_EDAARepository.GetFopEddaChild(referenceNo,cancellationToken);

            return Success(data);
        }

        [AuditAction("EXPORT_MT540_MSG")]
        [HttpGet("export_mt540_msg")]
        public async Task<IActionResult> ExportMT540Msg(int id, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true);

            FOPValidate data = await _fOP_EDAARepository.ExportMT540Msg(id, cancellationToken);

            if (data != null)
            {
                writer.WriteLine(data.PRS50_Req_Msg);
            }
            else
            {
                writer.WriteLine("Message is not available yet.");
            }

            writer.Flush();

            var fileName = $"I540_{_commonRepository.GetDateTimeForExportMsgs()}.txt";

            return File( memoryStream.ToArray(), "text/plain", fileName );
        }

        [AuditAction("SAVE_EDAA")]
        [HttpGet("save-edaa")]
        //Check User Can improve
        public async Task<IActionResult> SaveEDAA(string[] parmlist, CancellationToken cancellationToken)
        {
            ///////////////////////////////////////////111111111111111111111111 signalR remaining
            //var distinctArray = parmlist.Distinct().ToArray();
            //string vale = string.Join(",", distinctArray);
            //Msg _msg = new Msg();
            //_msg.Key = vale;
            //_msg.Message = "FOP";
            //_msg.MemberCode = User.MemberCode;
            //_msg.user = User.Id.ToString();
            //_msg.batchId = 0;
            //_Hubs.HubJunction.send_Notification_To_Fix(_msg);
            //_response.status = Enumerations.Status.Success;
            //_response.feedBack = "Request has been sent to Edda Successfully..";

            //return Success(data);

            return null;
        }
    }
}
