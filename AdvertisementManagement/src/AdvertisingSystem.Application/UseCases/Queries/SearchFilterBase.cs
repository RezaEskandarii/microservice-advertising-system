namespace AdvertisingSystem.Application.UseCases.Queries;

public abstract class SearchFilterBase
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 20;
    public string? Filter { get; set; }
}