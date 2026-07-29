using Asp.Versioning;
using Directfn.Custody.Api.Requests;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.PortfolioGroup;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Directfn.Custody.Api.Controllers
{
    [Authorize]
    [SkipEntitlement]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/portfolioGroup")]
    public class PortfolioGroupController : CustodyControllerBase
    {
        private readonly IPortfolioGroupRepository _portfolioGroupRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICustodyUserContext _custodyUserContext;
        public PortfolioGroupController(IPortfolioGroupRepository portfolioGroupRepository, ICommonRepository commonRepository, ICustodyUserContext custodyUserContext)
        {
            _portfolioGroupRepository = portfolioGroupRepository;
            _commonRepository = commonRepository;
            _custodyUserContext = custodyUserContext;
        }

        [AuditAction("GET_PORTFOLIO")]
        [HttpGet("get")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            int rd48_id = 1;// (int)_custodyUserContext.MemberCodeId;

            List<PortfolioGroupViewModel> data = await _portfolioGroupRepository.GetPortfolio(rd48_id, cancellationToken);

            return Success(data);

        }

        [AuditAction("GET_PORTFOLIO_BY_ID")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetByID(int um14_id, CancellationToken cancellationToken)
        {
            PortfolioGroupById data = await _portfolioGroupRepository.GetPortfolioByID(um14_id, cancellationToken);

            return Success(data);
        }

        [AuditAction("SAVE_PORTFOLIO")]
        [HttpPost("save")]
        public async Task<IActionResult> Save(PortfolioGroupReqModel portfolio, CancellationToken cancellationToken)
        {
            portfolio.um14_rf48_id = 1; // User.MemberCodeID; need to be change
            portfolio.um14_created_by = 1;// User.Id; need to be change

            if (portfolio.uploadedList.Count > 0)
            {
                string name = await _portfolioGroupRepository.GetCheckNameMembercodeISExists(portfolio.um14_group_name, portfolio.um14_rf48_id, cancellationToken);

                if (string.IsNullOrEmpty(name))
                {
                    int potfolio_id = await _portfolioGroupRepository.AddUpdatePortfolio(portfolio, cancellationToken);
                }
                else
                {
                    return Success(new { success = false, data = name });
                }
            }

            return Success(portfolio);
        }

        [AuditAction("UPDATE_PORTFOLIO")]
        [HttpPost("update")]
        public async Task<IActionResult> Update(PortfolioGroupReqModel portfolio, CancellationToken cancellationToken)
        {
            portfolio.um14_updated_by = 1;// User.Id; need to be change

            int potfolio_id = await _portfolioGroupRepository.AddUpdatePortfolio(portfolio, cancellationToken);

            return Success(portfolio);
        }


        [AuditAction("DELETE_PORTFOLIO")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(DeleteRequest request, CancellationToken cancellationToken)
        {
            //int user_id = Int32.Parse(_currentUserService.UserId);
            var data = await _portfolioGroupRepository.DeleteGroup(request.id, cancellationToken);

            return Success(data);
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> Upload_ExcelFile(IFormFile file, CancellationToken cancellationToken)
        {
            int rf48_id = 1; // User.MemberCodeID; need to be change
            int created_by = 1;// User.Id; need to be change

            List<PortfolioGroupValidate> Portfolio = new List<PortfolioGroupValidate>();

            DataSet result = _commonRepository.UploadFileDataSet(file);
            if (result != null && result.Tables != null && result.Tables[0].Rows.Count > 0)
            {
                int batch_id = await _commonRepository.GetBatchID("PORTFOLIO_GROUP", cancellationToken);

                var Portfolio2 = _portfolioGroupRepository.GetPortfolioGroup(result, batch_id, rf48_id, created_by, cancellationToken);

                bool is_sucess = _portfolioGroupRepository.BulkInsertPortfolioAccount(Portfolio2, cancellationToken).Result;

                if (is_sucess)
                {
                    Portfolio = await _portfolioGroupRepository.ValidatePortfolioAccounts(batch_id, rf48_id, cancellationToken);
                }
            }
            return Success(Portfolio);

        }
    }
}
