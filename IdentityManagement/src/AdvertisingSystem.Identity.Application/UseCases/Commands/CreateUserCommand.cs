using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AdvertisingSystem.Identity.Application.UseCases.Queries.Dtos;
using AdvertisingSystem.Identity.Domain.ValueObjects;
using MediatR;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class CreateUserCommand : IRequest<GetUser>
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Cell number is required.")]
    public string CellNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public Address? Address { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [JsonIgnore]
    public string? Role { get; set; }
}