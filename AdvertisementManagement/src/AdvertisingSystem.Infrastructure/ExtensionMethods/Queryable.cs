using AdvertisingSystem.Domain;
using Microsoft.EntityFrameworkCore;

namespace AdvertisingSystem.Infrastructure.ExtensionMethods;

public static class Queryable
{
    public static async Task<PaginatedList<T>> PaginateAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber > 0 ? pageNumber : 1;
        pageSize = pageSize > 0 ? pageSize : 20;

        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}