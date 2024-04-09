using System.ComponentModel.DataAnnotations;
using MediatR;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class LoginCommand : IRequest<LoginResponse>
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}