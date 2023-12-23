namespace AdvertisingSystem.Domain;

public class PaginatedList<T>
{
    public int PageNumber { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public List<T> Items { get; private set; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        Items = items;
    }

    public PaginatedList()
    {
        
    }
    public PaginatedList<T> Of(List<T> items)
    {
        this.Items = items;
        return this;
    }
}