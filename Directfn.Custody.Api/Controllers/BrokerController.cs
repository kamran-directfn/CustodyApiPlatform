using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.Banks;
using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Broker;
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
    [Route("api/v{version:apiVersion}/broker")]
    public class BrokerController : CustodyControllerBase
    {
        private readonly IBrokerRepository _brokerRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public BrokerController(IBrokerRepository brokerRepository, ICustodyUserContext custodyUserContext)
        {
            _brokerRepository = brokerRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_BROKER")]
        [HttpGet("get-broker")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            List<BrokerViewModel> data = await _brokerRepository.GetAllBrokersAsync(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_BROKER_BY_ID")]
        [HttpGet("get-broker-by-id")]
        public async Task<IActionResult> GetById(int brokerId, CancellationToken cancellationToken)
        {
            BrokerViewModel data = await _brokerRepository.GetBrokerById(brokerId, cancellationToken);

            return Success(data);
        }

        [AuditAction("APPROVE_BROKER")]
        [HttpPost("approve-broker")]
        // [RequireOperationApprovalCheck("user", "Um02_Id")]
        public async Task<IActionResult> Post(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _brokerRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("PENDING_BROKER")]
        [HttpPost("pending-broker")]
        public async Task<IActionResult> UnPost(PostUnpostRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _brokerRepository.UpdatePostStatus(request.id, request.Is_posted, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("DELETE_BROKER")]
        [HttpPost("delete-broker")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            int user_id = (int)_custodyUserContext.UserId;
            var data = await _brokerRepository.DeleteBroker(request.id, user_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_BROKER_CACHE_DATA")]
        [HttpGet("get-broker-cache-data")]
        public async Task<IActionResult> CacheBrokerData(CancellationToken cancellationToken)
        {
            List<BrokerCache> data = await _brokerRepository.CacheBrokerData(cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_BROKER_DATA")]
        [HttpPost("save-broker-data")]
        public async Task<IActionResult> Add(BrokerReqModel broker, CancellationToken cancellationToken)
        {
            BrokerReqModel data = await _brokerRepository.SaveBroker(broker, cancellationToken);

            return Success(data);
        }

        [AuditAction("UPDATE_BROKER_DATA")]
        [HttpPost("update-broker-data")]
        public async Task<IActionResult> Update(BrokerReqModel broker, CancellationToken cancellationToken)
        {
            BrokerReqModel data = await _brokerRepository.UpdateBroker(broker, cancellationToken);

            return Success(data);
        }

    }
}