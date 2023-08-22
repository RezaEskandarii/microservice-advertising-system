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

    public async Task<Advertisement> GetById(long id)
    {
        return await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Advertisement> AddAsync(Advertisement advertisement)
    {
        var result = await _context.Advertisements.AddAsync(advertisement);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Advertisement> Update(long id, Advertisement advertisement)
    {
       
        var entity = await _context.Advertisements.FirstOrDefaultAsync(x => x.Id == id);
        entity.Title = advertisement.Title;
        entity.Description = advertisement.Description;
        var result = _context.Advertisements.Update(entity);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<bool> Delete(long id)
    {
        var entity = await _context.Advertisements.FindAsync(id);
        if (entity != null) _context.Advertisements.Remove(entity);
        return true;
    }
}