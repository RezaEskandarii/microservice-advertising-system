using System.ComponentModel.DataAnnotations;
using MediatR;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class RefreshTokenCommand : IRequest<LoginResponse>
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; }
}