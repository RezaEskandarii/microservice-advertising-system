using AdvertisingSystem.Identity.Shared.Filters;

namespace AdvertisingSystem.Identity.Shared;

public class PaginatedResult<T>
{
    public PaginatedResult()
    {
    }

    public PaginatedResult(BaseFilter filter)
    {
        PageNumber = filter.PageNumber;
        PageSize = filter.PageSize;
    }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
    public ICollection<T> Items { get; set; }
}