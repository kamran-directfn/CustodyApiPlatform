using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Common.Enumerations;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Repositories.Common;
using Directfn.Custody.ApiFramework.Repositories.User;
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
    [Route("api/v{version:apiVersion}/common")]
    public sealed class CommonController : CustodyControllerBase
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IUserRepository _userRepository;
        public CommonController(ICommonRepository commonRepository, IUserRepository userRepository)
        {
            _commonRepository = commonRepository;
            _userRepository = userRepository;
        }

        [AuditAction("GET_ROLES")]
        [HttpGet("get_roles")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetRoles(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_MEMBER_CODES")]
        [HttpGet("get-member-codes")]
        public async Task<IActionResult> GetMemberCode(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetMemberCode(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SUPERVISORS")]
        [HttpGet("get-supervisors")]
        public async Task<IActionResult> getSupervisorDropdown(CancellationToken cancellationToken)
        {
            List<UserViewModel> user = await _userRepository.GetAllUserAsync(cancellationToken);

            List<DropDowns> data = user.AsEnumerable().Select(item => new DropDowns() { Id = item.UM02_ID.ToString(), text = item.UM02_NAME }).ToList();
            
            return Success(data);
        }

        [AuditAction("GET_PORTFOLIOS_BY_MEMBERS")]
        [HttpGet("get-portfolios-by-members")]
        public async Task<IActionResult> GetGroupsByMember(CancellationToken cancellationToken)
        {
            List<PortfoliosByMembers> data = await _commonRepository.GetGroupsByMember(cancellationToken);
          
            return Success(data);
        }

        [AuditAction("GET_BANK_DRP")]
        [HttpGet("get-banks")]
        public async Task<IActionResult> GetBanks(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetBanks(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_MARKET_DRP")]
        [HttpGet("get-markets")]
        public async Task<IActionResult> GetMarkets(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetMarkets(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_CURRENCY_DRP")]
        [HttpGet("get-currencies")]
        public async Task<IActionResult> GetCurrencies(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetCurrencies(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_COUNTRY_DRP")]
        [HttpGet("get-countries")]
        public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetCountries(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_ECONOMIC_SECTOR_DRP")]
        [HttpGet("get-economic-sector")]
        public async Task<IActionResult> GetEconomicSector(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetEconomicSector(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_MARKET_SECTOR_DRP")]
        [HttpGet("get-market-sector")]
        public async Task<IActionResult> GetMarketSector(CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetMarketSector(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_SUB_MARKET_DRP")]
        [HttpGet("get-sub-market")]
        public async Task<IActionResult> GetSubMarket(int marketId, CancellationToken cancellationToken)
        {
            List<DropDowns> data = await _commonRepository.GetSubMarket(marketId, cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_FOP_TRANSFER_TYPES_DRP")]
        [HttpGet("get-fop-transfer-types-dropdown")]
        public async Task<IActionResult> GetFopTransferTypesDropdown(CancellationToken cancellationToken)
        {
            List<DropDowns> data = _commonRepository.GetFopTransferTypesDropdown(cancellationToken); 

            return Success(data);
        }

        [AuditAction("GET_FOP_TRADE_TYPES_DRP")]
        [HttpGet("get-fop-trade-types-dropdown")]
        public async Task<IActionResult> GetTradeTypes(CancellationToken cancellationToken)
        {
            List<DropDowns> data = _commonRepository.GetTradeTypes(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_FOP_EDAA_STATUS_DRP")]
        [HttpGet("get-fop-edaa-status-dropdown")]
        public async Task<IActionResult> GetEdaaStatus(CancellationToken cancellationToken)
        {
            List<DropDowns> data = _commonRepository.GetEdaaStatus(cancellationToken);

            return Success(data);
        }

        [AuditAction("GET_AGENTS_DRP")]
        [HttpGet("get-agents-dropdown")]
        public async Task<IActionResult> GetAgents(CancellationToken cancellationToken)
        {
            List<DropDowns> data = _commonRepository.GetAgents(cancellationToken);

            return Success(data);
        }
    }
}
