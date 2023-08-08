namespace AdvertisingSystem.UserManagement.Shared;

public class PaginatedResult<T>
{
    public IList<T>? Data { get; set; }
}