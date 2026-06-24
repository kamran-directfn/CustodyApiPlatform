using System.ComponentModel.DataAnnotations;

namespace Directfn.Custody.Api.Requests.User;

public sealed class LoginRequest
{
    [Required]
    public string LoginId { get; init; } = default!;

    [Required]
    public string Password { get; init; } = default!;

    [Required]
    public string MemberCode { get; init; } = default!;
}