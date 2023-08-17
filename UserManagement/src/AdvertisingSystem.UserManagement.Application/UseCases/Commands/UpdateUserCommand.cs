using System.ComponentModel.DataAnnotations;

namespace AdvertisingSystem.UserManagement.Application.UseCases.Commands;

public class UpdateUserCommand
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Cell number is required.")]
    public string CellNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }
    
    public string? PhoneNumber { get; set; }
}