using System.Text.Json;
using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Infrastructure.Persistence.Repositories;

public class AdvertisementRepository : IAdvertisementRepository
{
    public Task<Advertisement> GetById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Advertisement> AddAsync(Advertisement advertisement)
    {
        Console.WriteLine(JsonSerializer.Serialize(advertisement));

        return null;
    }

    public Task<Advertisement> Update(long id, Advertisement advertisement)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(long id)
    {
        throw new NotImplementedException();
    }
}