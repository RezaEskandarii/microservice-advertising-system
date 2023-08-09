using System.ComponentModel.DataAnnotations;

namespace AdvertisingSystem.UserManagement.Contract.Dtos.User;

public class UpdatePasswordRequestDto
{
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}