namespace AdvertisingSystem.UserManagement.Shared;

public class PaginatedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
    public ICollection<T> Items { get; set; }
}