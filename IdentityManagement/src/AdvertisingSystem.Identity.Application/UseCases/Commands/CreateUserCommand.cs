using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AdvertisingSystem.Identity.Application.UseCases.Queries.Dtos;
using AdvertisingSystem.Identity.Domain.ValueObjects;
using MediatR;

namespace AdvertisingSystem.Identity.Application.UseCases.Commands;

public class CreateUserCommand : IRequest<GetUser>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNumber { get; set; }
    public Address? Address { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    [JsonIgnore] public string? Role { get; set; }
}