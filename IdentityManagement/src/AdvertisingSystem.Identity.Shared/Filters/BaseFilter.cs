namespace AdvertisingSystem.Identity.Shared.Filters;

public class BaseFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}