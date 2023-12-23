using AdvertisingSystem.Domain;
using AdvertisingSystem.Domain.Entities;

namespace AdvertisingSystem.Contract.Interfaces;

public interface IAdvertisementRepository
{
    Task<Advertisement> GetByIdAsync(long id);
    Task<Advertisement> GetByIdAsync(long id, string userId);
    Task<Advertisement> AddAsync(Advertisement advertisement);
    Task<Advertisement> UpdateAsync(long id, Advertisement advertisement);
    Task<Advertisement> UpdateAsync(Advertisement advertisement);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteAsync(long id, string userId);
    Task AddThumbnailAsync(long advertisementId, string thumbnailFileName);
    Task<PaginatedList<Advertisement>> SearchAsync(int pageNumber, int pageSize, string? requestFilter);
}