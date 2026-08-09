using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig;
using Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.HeadRoomConfig;
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
    [Route("api/v{version:apiVersion}/head-room-config")]
    [ApiController]
    public class HeadRoomConfigController : CustodyControllerBase
    {
        private readonly IHeadRoomConfigRepository _headRoomConfigRepository;    

        public HeadRoomConfigController(IHeadRoomConfigRepository headRoomConfigRepository)
        {
            _headRoomConfigRepository = headRoomConfigRepository;
        }

        [AuditAction("GET_HEAD_ROOM_CONFIGS")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<HeadRoomConfigViewModel> data = await _headRoomConfigRepository.GetHeadRoomConfigsAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_HEAD_ROOM_UPDATE")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(HeadRoomConfigReqModel req, CancellationToken cancellationToken)
        {
            HeadRoomConfigReqModel data = await _headRoomConfigRepository.UpdateHeadRoomConfig(req, cancellationToken);

            return Success(data);
        }
    }
}
