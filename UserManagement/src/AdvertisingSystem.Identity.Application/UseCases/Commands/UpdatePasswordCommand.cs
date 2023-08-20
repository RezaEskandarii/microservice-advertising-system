using System.ComponentModel.DataAnnotations;
using AdvertisingSystem.Identity.Application.UseCases.Queries.Dtos;
using MediatR;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class UpdatePasswordCommand : IRequest 
{
    public string userId;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}