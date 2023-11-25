namespace AdvertisingSystem.Contract.Interfaces;

public interface IElasticsearchRepository<T>
{
    Task<List<T>> SearchAsync(string searchText, int page, int pageSize);
}