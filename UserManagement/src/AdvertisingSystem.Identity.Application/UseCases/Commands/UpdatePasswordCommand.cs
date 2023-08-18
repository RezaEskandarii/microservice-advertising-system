using System.ComponentModel.DataAnnotations;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class UpdatePasswordCommand
{
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}