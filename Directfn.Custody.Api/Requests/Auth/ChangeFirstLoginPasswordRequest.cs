using System.ComponentModel.DataAnnotations;

namespace Directfn.Custody.Api.Requests.Auth;

public sealed class ChangeFirstLoginPasswordRequest
{
    [Required(ErrorMessage = "New password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public string NewPassword { get; init; } = default!;

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "New password and confirm password do not match.")]
    public string ConfirmPassword { get; init; } = default!;
}