using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Domain.Specifications;

public class AdvertisementSpecifications
{
    public bool IsFree(Advertisement advertisement)
    {
        return advertisement.Price.Amount == 0;
    }
}