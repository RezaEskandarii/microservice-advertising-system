using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Domain.Specifications;

public class AdvertisementSpecifications
{
    public bool IsFree(Advertisement advertisement)
    {
        if (advertisement.Price == null)
            return true;
        return advertisement.Price.Amount == 0;
    }
    
    
}