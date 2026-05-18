using Asp.Versioning;
using Directfn.Custody.ApiFramework.Controllers;
using Directfn.Custody.ApiFramework.Entitlements;
using Directfn.Custody.ApiFramework.Passwords;
using Microsoft.AspNetCore.Mvc;

namespace Directfn.Custody.SampleApi.Controllers;

[SkipEntitlement]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/password-test")]
public sealed class PasswordTestController : CustodyControllerBase
{
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILegacyPasswordService _legacyPasswordService;

    public PasswordTestController(
        IPasswordHashService passwordHashService,
        ILegacyPasswordService legacyPasswordService)
    {
        _passwordHashService = passwordHashService;
        _legacyPasswordService = legacyPasswordService;
    }

    [HttpPost("hash")]
    public IActionResult Hash([FromBody] PasswordHashTestRequest request)
    {
        var hash = _passwordHashService.HashPassword(request.Password);

        return Success(new
        {
            Hash = hash
        });
    }

    [HttpPost("verify")]
    public IActionResult Verify([FromBody] PasswordVerifyTestRequest request)
    {
        var result = _passwordHashService.VerifyPassword(
            request.PasswordHash,
            request.Password);

        return Success(new
        {
            Result = result.ToString(),
            IsValid = result == PasswordVerificationStatus.Success ||
                      result == PasswordVerificationStatus.SuccessRehashNeeded
        });
    }

    [HttpPost("verify-legacy")]
    public IActionResult VerifyLegacy([FromBody] LegacyPasswordVerifyTestRequest request)
    {
        var isValid = _legacyPasswordService.VerifyLegacyPassword(
            request.Password,
            request.LegacyEncryptedPassword);

        return Success(new
        {
            IsValid = isValid
        });
    }
}

public sealed class PasswordHashTestRequest
{
    public string Password { get; init; } = default!;
}

public sealed class PasswordVerifyTestRequest
{
    public string Password { get; init; } = default!;
    public string PasswordHash { get; init; } = default!;
}

public sealed class LegacyPasswordVerifyTestRequest
{
    public string Password { get; init; } = default!;
    public string LegacyEncryptedPassword { get; init; } = default!;
}