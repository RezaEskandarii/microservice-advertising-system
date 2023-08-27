using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IAdvertisementRepository
{
    Task<Advertisement> GetById(long id);
    Task<Advertisement> AddAsync(Advertisement advertisement);
    Task<Advertisement> Update(long id, Advertisement advertisement);
    Task<bool> Delete(long id);
    Task AddThumbnailAsync(long advertisementId, string thumbnailFileName);
}