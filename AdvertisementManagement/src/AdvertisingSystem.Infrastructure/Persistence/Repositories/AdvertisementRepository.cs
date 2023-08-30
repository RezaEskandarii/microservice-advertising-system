using AdvertisingSystem.Contract.Interfaces;
using AdvertisingSystem.Domain.Entities;
using AdvertisingSystem.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Infrastructure.Persistence.Repositories;

public class AdvertisementRepository : IAdvertisementRepository
{
    private readonly ApplicationDbContext _context;

    public AdvertisementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Advertisement> GetByIdAsync(long id)
    {
        return await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Advertisement> GetByIdAsync(long id, string userId)
    {
        var result = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (result == null)
            throw new Exception($"advertisement not found with id {id}");

        return result;
    }

    public async Task<Advertisement> AddAsync(Advertisement advertisement)
    {
        var result = await _context.Advertisements.AddAsync(advertisement);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Advertisement> UpdateAsync(long id, Advertisement advertisement)
    {
        var entity = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
        entity.Title = advertisement.Title;
        entity.Description = advertisement.Description;
        entity.Tags = advertisement.Tags;
        var result = _context.Advertisements.Update(entity);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Advertisement> UpdateAsync(Advertisement advertisement)
    {
        var result = _context.Advertisements.Update(advertisement);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await _context.Advertisements.FindAsync(id);
        if (entity != null) _context.Advertisements.Remove(entity);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new Exception("user id is null");

        var entity = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (entity == null)
            throw new Exception($"advertisement with id {id} is null");

        _context.Advertisements.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task AddThumbnailAsync(long advertisementId, string thumbnailFileName)
    {
        var advertisement = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == advertisementId);
        if (advertisement == null) return;

        if (advertisement.Thumbnails == null)
        {
            advertisement.Thumbnails = new[] { thumbnailFileName };
        }
        else
        {
            var thumbnails = new List<string>(advertisement.Thumbnails) { thumbnailFileName };
            advertisement.Thumbnails = thumbnails.ToArray();
        }

        _context.Advertisements.Update(advertisement);
        await _context.SaveChangesAsync();
    }
}