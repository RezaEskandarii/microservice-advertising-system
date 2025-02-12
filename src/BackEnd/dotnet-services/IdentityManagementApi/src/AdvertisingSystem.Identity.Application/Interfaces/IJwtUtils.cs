using AdvertisingSystem.Identity.Application.UseCases.Commands;
using AdvertisingSystem.Identity.Domain.Entities;

namespace AdvertisingSystem.Identity.Application.Interfaces;

public interface IJwtUtils
{
    public Task<LoginResponse> GenerateJwtTokenAsync(AppUser user);
}