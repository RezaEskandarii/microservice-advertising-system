using AdvertisingSystem.Identity.Shared.Filters;

namespace AdvertisingSystem.Identity.Shared.ExtensionMethods;

public static class IQuerableMethods
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageSize, int pageNumber)
    {
        // Calculate the number of items to skip based on the page size and number
        int itemsToSkip = (pageNumber - 1) * pageSize;

        // Apply the Skip and Take methods to perform pagination
        return query.Skip(itemsToSkip).Take(pageSize);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, BaseFilter filter)
    {
        // Calculate the number of items to skip based on the page size and number
        int itemsToSkip = (filter.PageNumber - 1) * filter.PageSize;

        // Apply the Skip and Take methods to perform pagination
        return query.Skip(itemsToSkip).Take(filter.PageSize);
    }
}