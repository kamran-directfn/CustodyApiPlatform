using Asp.Versioning;
using Directfn.Custody.ApiFramework.Auditing;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Menus;
using Directfn.Custody.ApiFramework.Repositories.User;
using Directfn.Custody.ApiFramework.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.Api.Controllers;

[Authorize]
[SkipEntitlement]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/menu")]
public sealed class MenuController : CustodyControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ICustodyUserContext _custodyUserContext;
    private readonly ILeftMenuBuilder _leftMenuBuilder;

    public MenuController(IUserRepository userRepository, ICustodyUserContext custodyUserContext, ILeftMenuBuilder leftMenuBuilder)
    {
        _userRepository = userRepository;
        _custodyUserContext = custodyUserContext;   
        _leftMenuBuilder = leftMenuBuilder;
    }

    [AuditAction("GET_LEFT_MENU")]
    [HttpGet("left")]
    public async Task<IActionResult> GetLeftMenu(CancellationToken cancellationToken)
    {
        if (!_custodyUserContext.IsAuthenticated)
        {
            return Unauthorized(new { Success = false, Message = "User is not authenticated." });
        }
        var entitlements = await _userRepository.GetUserEntitlementsAsync(_custodyUserContext.UserId, cancellationToken);

        var menu = _leftMenuBuilder.Build(entitlements);

        return Success(menu);
    }
}